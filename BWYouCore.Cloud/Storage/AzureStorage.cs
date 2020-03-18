using BWYouCore.Cloud.Exceptions;
using HeyRed.Mime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Cloud.Storage
{
    /// <summary>
    /// AzureStorage
    /// Microsoft.WindowsAzure.Storage Nuget 설치 필수!!
    /// </summary>
    public class AzureStorage : IStorage
    {
        /// <summary>
        /// AzureStorage Account
        /// </summary>
        public CloudStorageAccount storageAccount { get; set; }

        /// <summary>
        /// storageAccount 설정 필수
        /// </summary>
        public AzureStorage()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionString"></param>
        public AzureStorage(string connectionString)
        {
            this.storageAccount = CloudStorageAccount.Parse(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storageAccount"></param>
        public AzureStorage(CloudStorageAccount storageAccount)
        {
            this.storageAccount = storageAccount;
        }
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
        public async Task<string> UploadAsync(Stream inputStream, string sourcefilename, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            CloudBlockBlob blockBlob = await GetCloudBlockBlobAsync(sourcefilename, container, destpath, useUUIDName, overwrite, useSequencedName);

            using (var fileStream = inputStream)
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }

            string mimeType = MimeTypesMap.GetMimeType(sourcefilename);
            blockBlob.Properties.ContentType = mimeType;
            await blockBlob.SetPropertiesAsync();

            return blockBlob.StorageUri.PrimaryUri.AbsoluteUri;
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
        public async Task<string> UploadAsync(string sourcefilepathname, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            FileInfo fileInfo = new FileInfo(sourcefilepathname);

            FileStream fileStream = null;
            try
            {
                fileStream = fileInfo.OpenRead();
                var uri = await UploadAsync(fileStream, fileInfo.Name, containerName, destpath, useUUIDName, overwrite, useSequencedName);
                return uri;
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// 요구 사항에 맞는 이름으로 된 BlockBlob 획득
        /// </summary>
        /// <param name="sourcefilename">원본 파일 명</param>
        /// <param name="container">컨테이너</param>
        /// <param name="destpath">파일 저장 패스</param>
        /// <param name="useUUIDName">UUID 이름 사용 여부. false일 경우는 filename과 동일 한 이름으로 저장</param>
        /// <param name="overwrite">동일 이름 존재 시 덮어쓰기 여부. useUUIDName 사용 중일 경우는 항상 덮어쓰기 안 함</param>
        /// <param name="useSequencedName">동일 이름 존재 시 순차적인 이름 사용 여부. 파일[0].확장자, 파일[1].확장자.. </param>
        /// <returns></returns>
        private async Task<CloudBlockBlob> GetCloudBlockBlobAsync(string sourcefilename, CloudBlobContainer container, string destpath, bool useUUIDName, bool overwrite, bool useSequencedName)
        {
            FileInfo fileInfo = new FileInfo(sourcefilename);
            string destfilename = fileInfo.Name;

            if (useUUIDName)
            {
                while (true)
                {

                    destfilename = Guid.NewGuid().ToString().Replace("-", "") + fileInfo.Extension;
                    string path = Path.Combine(destpath, destfilename);
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path.Replace(@"\", "/"));    //OS 또는 언어?에 따라 %5c로 path가 나타나는 버그가 존재하여 미리 변경하여 처리
                    if (await blockBlob.ExistsAsync())
                    {
                        continue;
                    }
                    else
                    {
                        return blockBlob;
                    }
                }

            }
            else
            {

                if (overwrite)
                {
                    string path = Path.Combine(destpath, destfilename);
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(path.Replace(@"\", "/"));

                    return blockBlob;
                }
                else
                {
                    string destfilenameRe = destfilename;

                    string filename = destfilename.Substring(0, destfilename.Length - fileInfo.Extension.Length);

                    uint i = 0;
                    while (true)
                    {
                        string path = Path.Combine(destpath, destfilenameRe);
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference(path.Replace(@"\", "/"));
                        if (await blockBlob.ExistsAsync() == true)
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
                            return blockBlob;
                        }
                    }
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
        public async Task<string> DownloadAsync(Uri sourceUri, string destfilename, bool overwrite = false, bool useSequencedName = true)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            ICloudBlob blob = await blobClient.GetBlobReferenceFromServerAsync(sourceUri);
            if (overwrite == true)
            {
                await blob.DownloadToFileAsync(destfilename, FileMode.Create);
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
                        await blob.DownloadToFileAsync(destfilenameRe, FileMode.CreateNew);
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
        /// <summary>
        /// 스토리지 파일을 스트림에 다운로드
        /// </summary>
        /// <param name="sourceUri"></param>
        /// <param name="deststream"></param>
        /// <returns></returns>
        public async Task<string> DownloadAsync(Uri sourceUri, Stream deststream)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            ICloudBlob blob = await blobClient.GetBlobReferenceFromServerAsync(sourceUri);
            await blob.DownloadToStreamAsync(deststream);
            return "";
        }
        /// <summary>
        /// 스토리지 파일 제거
        /// </summary>
        /// <param name="sourceUri"></param>
        public async Task DeleteAsync(Uri sourceUri)
        {
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            ICloudBlob blob = await blobClient.GetBlobReferenceFromServerAsync(sourceUri);
            await blob.DeleteAsync();
        }
    }
}
