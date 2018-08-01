using BWYouCore.Cloud.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Cloud.Storage
{
    public class FileStorage : IStorage
    {
        /// <summary>
        /// 업로드 중 문제 발생 시 재시도 횟수
        /// </summary>
        public int retryCount { get; set; }
        /// <summary>
        /// 파일 저장 기본 루트 위치
        /// </summary>
        public string RootPath { get; set; }
        /// <summary>
        /// 외부에 보여 질 루트 Url
        /// </summary>
        public string PublicRootUrl { get; set; }

        public FileStorage(string rootPath, string publicRootUrl)
        {
            this.retryCount = 2;
            this.RootPath = rootPath;
            this.PublicRootUrl = publicRootUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="sourcefilename"></param>
        /// <param name="containerName"></param>
        /// <param name="destpath"></param>
        /// <param name="useUUIDName"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <returns></returns>
        public async Task<string> UploadAsync(System.IO.Stream inputStream, string sourcefilename, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            do
            {
                try
                {
                    string relativeFilePath = GetDestFilePath(sourcefilename, containerName, destpath, useUUIDName, overwrite, useSequencedName);
                    InitDirectory(Path.Combine(RootPath, relativeFilePath));

                    using (var fileStream = File.Create(Path.Combine(RootPath, relativeFilePath)))
                    {
                        inputStream.Seek(0, SeekOrigin.Begin);
                        await inputStream.CopyToAsync(fileStream);
                    }

                    Uri url = new Uri(PublicRootUrl + "/" + relativeFilePath);
                    return url.AbsoluteUri;
                }
                catch (Exception ex)
                {
                    if (typeof(DuplicateFileException) == ex.GetType())
                    {
                        throw ex;
                    }
                }
            } while (--retryCount > 0);

            throw new OutOfReTryCountException();
        }
        /// <summary>
        /// 생성 할 파일의 부모 디렉토리 존재 여부 확인 후 없으면 생성
        /// </summary>
        /// <param name="createFilePath"></param>
        private void InitDirectory(string createFilePath)
        {
            DirectoryInfo diDesc = new DirectoryInfo((new FileInfo(createFilePath)).DirectoryName);
            if (diDesc.Exists == false)
            {
                Directory.CreateDirectory(diDesc.FullName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcefilepathname"></param>
        /// <param name="containerName"></param>
        /// <param name="destpath"></param>
        /// <param name="useUUIDName"></param>
        /// <param name="overwrite"></param>
        /// <param name="useSequencedName"></param>
        /// <returns></returns>
        public Task<string> UploadAsync(string sourcefilepathname, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true)
        {
            FileInfo fileInfo = new FileInfo(sourcefilepathname);

            using (var fileStream = fileInfo.OpenRead())
            {
                return UploadAsync(fileStream, fileInfo.Name, containerName, destpath, useUUIDName, overwrite, useSequencedName);
            }
        }

        private string GetDestFilePath(string sourcefilename, string containerName, string destpath, bool useUUIDName, bool overwrite, bool useSequencedName)
        {
            FileInfo fileInfo = new FileInfo(sourcefilename);
            string destfilename = fileInfo.Name;

            if (useUUIDName)
            {
                while (true)
                {

                    destfilename = Guid.NewGuid().ToString().Replace("-", "") + fileInfo.Extension;

                    string filePath = Path.Combine(RootPath, containerName, destpath, destfilename);

                    if (File.Exists(filePath) == true)
                    {
                        continue;
                    }
                    else
                    {
                        return Path.Combine(containerName, destpath, destfilename); //상대 저장 위치만 반환
                    }
                }
            }
            else
            {
                return GetDestFilePath(Path.Combine(containerName, destpath), destfilename, overwrite, useSequencedName, RootPath);
            }
        }

        private string GetDestFilePath(string destpath, string destfilename, bool overwrite, bool useSequencedName, string destRootPath = "")
        {
            if (overwrite)
            {
                return Path.Combine(destpath, destfilename);    //destRootPath 제외한 저장 위치만 반환
            }
            else
            {
                FileInfo fileInfo = new FileInfo(destfilename);
                string destfilenameRe = destfilename;

                string filename = destfilename.Substring(0, destfilename.Length - fileInfo.Extension.Length);

                uint i = 0;
                while (true)
                {
                    string filePath = Path.Combine(destRootPath, destpath, destfilenameRe);

                    if (File.Exists(filePath) == true)
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
                        return Path.Combine(destpath, destfilenameRe);  //destRootPath 제외한 저장 위치만 반환
                    }
                }
            }
        }

        public async Task<string> DownloadAsync(Uri sourceUri, string destfilename, bool overwrite = false, bool useSequencedName = true)
        {
            string destFilePathName = GetDestFilePath("", destfilename, overwrite, useSequencedName);
            InitDirectory(destFilePathName);

            using (var fileStream = File.Create(destFilePathName))
            {
                await DownloadAsync(sourceUri, fileStream);
                return destFilePathName;
            }

        }

        public async Task<string> DownloadAsync(Uri sourceUri, System.IO.Stream deststream)
        {
            string realPath = GetRealPathFromUri(sourceUri);

            using (var fileStream = File.OpenRead(realPath))
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                fileStream.CopyTo(deststream);
            }
            return "";
        }

        public Task DeleteAsync(Uri sourceUri)
        {
            string realPath = GetRealPathFromUri(sourceUri);
            return Task.Factory.StartNew(() => File.Delete(realPath));
        }

        private string GetRealPathFromUri(Uri sourceUri)
        {
            if (sourceUri.AbsoluteUri.StartsWith(PublicRootUrl) == false)
            {
                throw new InvalidDataException();
            }

            return Path.Combine(RootPath, sourceUri.AbsolutePath.Substring((new Uri(PublicRootUrl)).AbsolutePath.Length + 1));
        }
    }
}
