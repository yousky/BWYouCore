using BWYouCore.Cloud.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Cloud.Storage
{
    /// <summary>
    /// UCloudStorage Http 이용 처리
    /// </summary>
    public class UCloudStorage : IStorage
    {
        //TODO Singleton을 굳이 써야 할까?
        #region For Singleton
        private static readonly UCloudStorage __singleton = new UCloudStorage();
        static UCloudStorage()
        {
            retryCount = 2;
        }
        /// <summary>
        /// static Singleton Instance
        /// </summary>
        public static UCloudStorage Instance
        {
            get
            {
                return __singleton;
            }
        }
        #endregion

        /// <summary>
        /// 인증 Url
        /// </summary>
        public string authUrl { get; set; }
        /// <summary>
        /// 스토리지 유저 아이디
        /// </summary>
        public string storageUser { get; set; }
        /// <summary>
        /// 스토리지 유저 패스
        /// </summary>
        public string storagePass { get; set; }
        /// <summary>
        /// 업로드 중 문제 발생 시 재시도 횟수
        /// </summary>
        public static int retryCount { get; set; }

        private string _AuthToken = "";
        private string _StorageUrl = "";
        private DateTime _AuthTokenExpiresDateTime = DateTime.MinValue;

        /// <summary>
        /// 스트림을 스토리지에 업로드
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="sourcefilename"></param>
        /// <param name="containerName"></param>
        /// <param name="destpath"></param>
        /// <param name="useUUIDName"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <returns></returns>
        public async Task<UploadedInfo> UploadAsync(Stream inputStream, string sourcefilename, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            bool forceRequestAuth = false;

            int retryCount = UCloudStorage.retryCount;

            do
            {
                string authToken = await GetAuthTokenAsync(forceRequestAuth);

                if (string.IsNullOrEmpty(authToken) == true)
                {
                    continue;
                }

                try
                {
                    Uri url = await GetDestFileUrlAsync(sourcefilename, containerName, destpath, useUUIDName, overwrite, useSequencedName, authToken);

                    var uploadedInfo = await UploadFromStreamAsync(inputStream, url, authToken);

                    return uploadedInfo;
                }
                catch (HttpWebResponseUnauthorizedException)
                {
                    forceRequestAuth = true;
                }
                catch (Exception)
                {

                }

            } while (--retryCount > 0);

            throw new OutOfReTryCountException();

        }

        private async Task<UploadedInfo> UploadFromStreamAsync(Stream inputStream, Uri url, string authToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "PUT";
            request.Headers.Add("X-Auth-Token", authToken);
            request.ContentLength = inputStream.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            using (System.IO.Stream requestStream = request.GetRequestStream())
            {
                inputStream.Position = 0;
                await inputStream.CopyToAsync(requestStream);
            }

            using (HttpWebResponse response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    return new UploadedInfo { AbsoluteUri = url.AbsoluteUri, Length = inputStream.Length };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpWebResponseUnauthorizedException();
                }
                else
                {
                    throw new HttpWebResponseException();
                }
            }
        }

        /// <summary>
        /// 원본 파일을 스토리지에 업로드
        /// </summary>
        /// <param name="sourcefilepathname"></param>
        /// <param name="containerName"></param>
        /// <param name="destpath"></param>
        /// <param name="useUUIDName"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <returns></returns>
        public Task<UploadedInfo> UploadAsync(string sourcefilepathname, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            FileInfo fileInfo = new FileInfo(sourcefilepathname);

            using (var fileStream = fileInfo.OpenRead())
            {
                return UploadAsync(fileStream, fileInfo.Name, containerName, destpath, useUUIDName, overwrite, useSequencedName);
            }
        }

        /// <summary>
        /// 요구 사항에 맞는 이름의 Url 획득
        /// </summary>
        /// <param name="sourcefilename"></param>
        /// <param name="containerName"></param>
        /// <param name="destpath"></param>
        /// <param name="useUUIDName"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private async Task<Uri> GetDestFileUrlAsync(string sourcefilename, string containerName, string destpath, bool useUUIDName, bool overwrite, bool useSequencedName, string authToken)
        {
            FileInfo fileInfo = new FileInfo(sourcefilename);
            string destfilename = fileInfo.Name;

            string storageUrl = _StorageUrl;

            if (useUUIDName)
            {
                while (true)
                {

                    destfilename = Guid.NewGuid().ToString().Replace("-", "") + fileInfo.Extension;

                    Uri url = new Uri(storageUrl + "/" + Path.Combine(containerName, destpath, destfilename));

                    if (await UCloudFileExistAsync(url, authToken) == true)
                    {
                        continue;
                    }
                    else
                    {
                        return url;
                    }
                }
            }
            else
            {
                if (overwrite)
                {
                    return new Uri(storageUrl + "/" + Path.Combine(containerName, destpath, destfilename));
                }
                else
                {
                    string destfilenameRe = destfilename;

                    string filename = destfilename.Substring(0, destfilename.Length - fileInfo.Extension.Length);

                    uint i = 0;
                    while (true)
                    {
                        Uri url = new Uri(storageUrl + "/" + Path.Combine(containerName, destpath, destfilenameRe));

                        if (await UCloudFileExistAsync(url, authToken) == true)
                        {
                            if (useSequencedName == true)
                            {
                                i++;
                                destfilenameRe = filename + "[" + i.ToString() + "]" + fileInfo.Extension;
                                continue;
                            }
                            else
                            {
                                throw new DuplicateFileException();
                            }
                        }
                        else
                        {
                            return url;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 해당 주소에 대한 GET HttpStatusCode OK 확인
        /// 유클라우드는 웹에서 존재 하기 때문에 이를 이용하여 파일 존재 여부 체크 가능
        /// </summary>
        /// <param name="url"></param>
        /// <param name="authToken"></param>
        /// <returns></returns>
        private async Task<bool> UCloudFileExistAsync(Uri url, string authToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";    //Todo 이렇게 찾지 않고 Head 값으로 찾을 수도 있을 거 같은데.. ~_~?
            request.Headers.Add("X-Auth-Token", authToken);

            using (HttpWebResponse response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpWebResponseUnauthorizedException();
                }
                else
                {
                    throw new HttpWebResponseException();
                }
            }
        }
        /// <summary>
        /// 스토리지 파일을 파일로 다운로드
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="destfilename"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <returns></returns>
        public Task<string> DownloadAsync(Uri sourceUri, string destfilename, bool overwrite = false, bool useSequencedName = true)
        {
            return TryDownloadAsync<string>(sourceUri, destfilename, overwrite, useSequencedName, WebDownloadAsync);
        }
        /// <summary>
        /// 스토리지 파일을 스트림에 다운로드
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="deststream"></param>
        /// <returns></returns>
        public Task<string> DownloadAsync(Uri sourceUri, Stream deststream)
        {
            return TryDownloadAsync<Stream>(sourceUri, deststream, false, false, WebDownloadAsync);
        }

        private async Task<string> TryDownloadAsync<T>(Uri sourceUri, T dest, bool overwrite, bool useSequencedName, Func<Uri, T, string, bool, bool, Task<string>> func)
        {
            bool forceRequestAuth = false;

            int retryCount = UCloudStorage.retryCount;

            do
            {
                string authToken = await GetAuthTokenAsync(forceRequestAuth);

                if (string.IsNullOrEmpty(authToken) == true)
                {
                    continue;
                }

                try
                {
                    return await func(sourceUri, dest, authToken, overwrite, useSequencedName);
                }
                catch (HttpWebResponseUnauthorizedException)
                {
                    forceRequestAuth = true;
                }
                catch (Exception)
                {

                }

            } while (--retryCount > 0);

            throw new OutOfReTryCountException();
        }

        private async Task<string> WebDownloadAsync(Uri sourceUri, string destfilename, string authToken, bool overwrite, bool useSequencedName)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("X-Auth-Token", authToken);

            if (overwrite == true)
            {
                await webClient.DownloadFileTaskAsync(sourceUri, destfilename);
                FileInfo fi = new FileInfo(destfilename);
                if (fi.Exists)
                {
                    return fi.FullName;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                FileInfo fileInfo = new FileInfo(destfilename);
                string destfilenameRe = destfilename;

                string filename = destfilename.Substring(0, destfilename.Length - fileInfo.Extension.Length);

                uint i = 0;
                while (true)
                {
                    if (File.Exists(destfilenameRe) == true)
                    {
                        if (useSequencedName == true)
                        {
                            i++;
                            destfilenameRe = filename + "[" + i.ToString() + "]" + fileInfo.Extension;
                            continue;
                        }
                        else
                        {
                            throw new DuplicateFileException();
                        }
                    }
                    else
                    {
                        await webClient.DownloadFileTaskAsync(sourceUri, destfilenameRe);
                        FileInfo fi = new FileInfo(destfilenameRe);
                        if (fi.Exists)
                        {
                            return fi.FullName;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        private async Task<string> WebDownloadAsync(Uri sourceUri, Stream deststream, string authToken, bool overwrite, bool useSequencedName)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceUri);
            request.Method = "GET";
            request.Headers.Add("X-Auth-Token", authToken);

            using (HttpWebResponse response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new HttpWebResponseException();
                }
                else if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        var buffer = new byte[4096];
                        long totalBytesRead = 0;
                        int bytesRead;

                        while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead += bytesRead;
                            await deststream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                    return "";
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpWebResponseUnauthorizedException();
                }
                else
                {
                    throw new HttpWebResponseException();
                }
            }
        }

        /// <summary>
        /// request에 대한 response 획득
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            HttpWebResponse response = null;
            try
            {
                response = (HttpWebResponse)await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
            }
            return response;
        }

        /// <summary>
        /// 인증
        /// </summary>
        /// <returns></returns>
        private async Task<string> AuthAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(authUrl);
            request.Method = "GET";    // 기본값 "GET"

            request.Headers.Add("X-Storage-User", storageUser);
            request.Headers.Add("X-Storage-Pass", storagePass);
            request.Headers.Add("X-Auth-New-Token", "true");

            using (HttpWebResponse response = await GetResponseAsync(request))
            {
                if ((response.StatusCode != HttpStatusCode.OK) || response.Headers["X-Auth-Token"] == null || response.Headers["X-Storage-Url"] == null || response.Headers["X-Auth-Token-Expires"] == null)
                {
                    return null;
                }
                _AuthToken = response.Headers["X-Auth-Token"];
                _StorageUrl = response.Headers["X-Storage-Url"];
                _AuthTokenExpiresDateTime = DateTime.Now.AddSeconds(double.Parse(response.Headers["X-Auth-Token-Expires"]) - 3600); //만료시간 보단 1시간 정도 미리 만료 처리하자.
            }

            return _AuthToken;
        }

        /// <summary>
        /// 인증 토큰 획득
        /// </summary>
        /// <param name="forceRequest"></param>
        /// <returns></returns>
        private async Task<string> GetAuthTokenAsync(bool forceRequest = false)
        {
            if (forceRequest || (DateTime.Now > _AuthTokenExpiresDateTime))
            {
                return await AuthAsync();
            }
            return _AuthToken;
        }

        /// <summary>
        /// 스토리지 파일 제거.
        /// </summary>
        /// <param name="sourceUri"></param>
        public async Task DeleteAsync(Uri sourceUri)
        {
            bool forceRequestAuth = false;

            int retryCount = UCloudStorage.retryCount;

            do
            {
                string authToken = await GetAuthTokenAsync(forceRequestAuth);

                if (string.IsNullOrEmpty(authToken) == true)
                {
                    continue;
                }

                try
                {
                    await DeleteAsync(sourceUri, authToken);
                    return;
                }
                catch (HttpWebResponseUnauthorizedException)
                {
                    forceRequestAuth = true;
                }
                catch (Exception)
                {

                }

            } while (--retryCount > 0);

            throw new OutOfReTryCountException();
        }

        private async Task DeleteAsync(Uri sourceUri, string authToken)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sourceUri);
            request.Method = "DELETE";
            request.Headers.Add("X-Auth-Token", authToken);
            using (HttpWebResponse response = await GetResponseAsync(request))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return; // 못 찾는 것도 지운 걸로 처리
                }
                else if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpWebResponseUnauthorizedException();
                }
                else
                {
                    throw new HttpWebResponseException();
                }
            }

        }
    }
}
