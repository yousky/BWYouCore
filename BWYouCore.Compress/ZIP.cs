using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BWYouCore.Compress
{
    /// <summary>
    /// ZIP 형식의 압축 클래스
    /// </summary>
    public class ZIP
    {
        /// <summary>
        /// 마지막 메세지 저장용
        /// </summary>
        string strLastMessage = "";
        /// <summary>
        /// 마지막 메세지를 외부에 노출
        /// </summary>
        public string StrLastMessage
        {
            get { return strLastMessage; }
            //set { strLastMessage = value; }
        }

        /// <summary>
        /// 1개의 파일 또는 1개의 폴더를 특정폴더+파일명으로 zip 압축하기
        /// </summary>
        /// <param name="strDestination_file_path_name_Zip">압축 될 zip 파일의 패스 및 이름</param>
        /// <param name="strSource_file_path_name">압축 할 파일의 패스 및 이름</param>
        /// <returns>true : 압축정상 성공, false : 압축 할 파일 없음, throw : 에러 발생</returns>
        public bool Compress(string strDestination_file_path_name_Zip, string strSource_file_path_name)
        {
            try
            {
                ArrayList arSources;
                int TrimLength = 0;

                //디렉토리인지 파일인지 여부 확인.
                if (IsDirectory(strSource_file_path_name) == true)
                {
                    // 압축 대상 폴더의 파일 목록.
                    arSources = GenerateFileFolderList(strSource_file_path_name);

                    // 압축 대상 폴더 경로의 길이 + 1
                    TrimLength = (Directory.GetParent(strSource_file_path_name)).ToString().Length + 1;

                    // find number of chars to remove. from orginal file path. remove '\'
                }
                else
                {
                    FileInfo fi = new FileInfo(strSource_file_path_name);
                    if (fi.Exists == false)
                    {
                        strLastMessage = "File Not Found";
                        return false;
                    }
                    else
                    {
                        arSources = new ArrayList();
                        arSources.Add(strSource_file_path_name);

                        TrimLength = fi.DirectoryName.Length + 1;
                    }
                }

                //압축 하기
                Compress(strDestination_file_path_name_Zip, arSources, TrimLength);

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// 1개의 파일을 특정폴더+파일명으로 zip 압축하기
        /// </summary>
        /// <param name="strDestination_file_path_name_Zip">압축 될 zip 파일의 패스 및 이름</param>
        /// <param name="arSources">압축 할 파일 및 폴더들</param>
        /// <param name="TrimLength">압축 파일 내에서의 상대 위치 설정을 위한 trim 값. -1일 경우 1단계 파일명으로만 압축 되도록 처리</param>
        /// <returns>true : 압축정상 성공, false : 압축 할 파일 없음, throw : 에러 발생</returns>
        public void Compress(string strDestination_file_path_name_Zip, ArrayList arSources, int TrimLength)
        {

            try
            {
                //strDestination_file_path 폴더 없을 경우 만들기(폴더가 중간경로에도 없더라도 다 만들어줌)
                FileInfo fiDest_File = new FileInfo(strDestination_file_path_name_Zip);
                if (fiDest_File.Directory.Exists == false)
                {
                    Directory.CreateDirectory(fiDest_File.Directory.FullName);
                }

                // 압축된 파일용 파일스트림을 연결 
                using (ZipOutputStream Zipoutput = new ZipOutputStream(File.Create(strDestination_file_path_name_Zip)))
                {
                    OnCompressStart(0, 1, "Compress_ZIP Start");

                    //Zipoutput.SetComment("");   // 코멘트
                    //Zipoutput.SetLevel(9);      // 0 - store only to 9 - means best compression

                    byte[] buffer = new byte[4096];

                    long nTotalWorkBytes = 0;

                    foreach (string fiSrc in arSources)
                    {

                        // ZipEntry 등록
                        string fiSrcZip;
                        if (TrimLength < 0)
                        {
                            if (IsDirectory(fiSrc) == true)
                            {
                                fiSrcZip = new DirectoryInfo(fiSrc).Name;
                            }
                            else
                            {
                                fiSrcZip = new FileInfo(fiSrc).Name;
                            }
                        }
                        else
                        {
                            fiSrcZip = fiSrc.Remove(0, TrimLength);
                        }

                        ZipEntry zeEntry = new ZipEntry(fiSrcZip);
                        zeEntry.DateTime = DateTime.Now;

                        if (IsDirectory(fiSrc) == false)
                        {
                            FileInfo fiInfoSrc = new FileInfo(fiSrc);
                            zeEntry.Size = fiInfoSrc.Length;

                            Zipoutput.PutNextEntry(zeEntry);

                            // 파일의 스트림을 가져와서            
                            using (FileStream fsSource = fiInfoSrc.OpenRead())
                            {
                                long nWorkBytes = 0;
                                long nSourceLength = fsSource.Length;

                                OnCompressProgress(nWorkBytes, nSourceLength, "Compress_ZIP Working [" + fiSrc + "] compressiong");

                                // 모두 읽어서 Zip 스트림에다가 그대로 넣어준다.
                                int nSourceBytes;
                                int nWhile = 0;
                                do
                                {
                                    nSourceBytes = fsSource.Read(buffer, 0, buffer.Length);
                                    Zipoutput.Write(buffer, 0, nSourceBytes);

                                    nWorkBytes = nWorkBytes + nSourceBytes;

                                    nWhile++;
                                    if (nWhile % 10 == 0)
                                    {
                                        OnCompressProgress(nWorkBytes, nSourceLength, "Compress_ZIP Working");
                                    }


                                } while (nSourceBytes > 0);

                                OnCompressProgress(nWorkBytes, nSourceLength, "Compress_ZIP Working [" + fiSrc + "] compressed");

                                nTotalWorkBytes = nTotalWorkBytes + nWorkBytes;
                            }
                        }
                        else
                        {
                            Zipoutput.PutNextEntry(zeEntry);
                        }
                    }

                    Zipoutput.Finish();
                    Zipoutput.Close();

                    OnCompressEnd(nTotalWorkBytes, nTotalWorkBytes, "Compress_ZIP End");

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private bool IsDirectory(string path)
        {
            if ((File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// blog.naver.com/vackjangmi?Redirect=Log&logNo=120126808831
        /// 파일, 폴더 목록 생성
        /// </summary>
        /// <param name="Dir">폴더 경로</param>
        /// <returns>폴더, 파일 목록(ArrayList)</returns>
        private ArrayList GenerateFileFolderList(string Dir)
        {
            ArrayList fils = new ArrayList();

            bool Empty = true;

            // 폴더 내의 파일 추가.
            foreach (string file in Directory.GetFiles(Dir))
            {
                fils.Add(file);
                Empty = false;
            }

            if (Empty)
            {
                // 파일이 없고, 폴더도 없는 경우 자신의 폴더 추가.
                if (Directory.GetDirectories(Dir).Length == 0)
                    fils.Add(Dir + Path.DirectorySeparatorChar.ToString()); //구분자를 OS에 따라 변경 되도록 수정 by you 20130422
            }

            // 폴더 내 폴더 목록.
            foreach (string dirs in Directory.GetDirectories(Dir))
            {
                // 해당 폴더로 다시 GenerateFileList 재귀 호출
                foreach (object obj in GenerateFileFolderList(dirs))
                {
                    // 해당 폴더 내의 파일, 폴더 추가.
                    fils.Add(obj);
                }
            }
            return fils;

        }





        /// <summary>
        /// 특정폴더에다가 압축(zip) 파일 해제하기
        /// </summary>
        /// <param name="strDestination_file_path">압축이 풀릴 패스</param>
        /// <param name="strSource_file_path_name_Zip">압축을 풀 zip 파일 패스 및 이름</param>
        /// <param name="lsDecompressedFilesPathName">압축이 풀린 파일의 목록</param>
        /// <returns>true : 압축 풀기 성공, false : 압축 풀 파일 없음, throw : 에러 발생</returns>
        public bool DeCompress(string strDestination_file_path, string strSource_file_path_name_Zip, out List<string> lsDecompressedFilesPathName)
        {

            try
            {
                lsDecompressedFilesPathName = new List<string>();


                //strSource_file  파일 존재하나?
                FileInfo fiSource_File = new FileInfo(strSource_file_path_name_Zip);
                if (fiSource_File.Exists == false)
                {
                    strLastMessage = "File Not Found";
                    return false;
                }

                long nSource_FileLength = fiSource_File.Length;

                //strDestination_file_path 폴더 없을 경우 만들기(폴더가 중간경로에도 없더라도 다 만들어줌)
                DirectoryInfo diDesc = new DirectoryInfo(strDestination_file_path);
                if (diDesc.Exists == false)
                {
                    Directory.CreateDirectory(strDestination_file_path);
                }

                // 압축된 파일용 파일스트림을 연결 
                using (ZipInputStream Zipinput = new ZipInputStream(File.OpenRead(strSource_file_path_name_Zip)))
                {
                    long nTotalWorkBytes = 0;
                    OnCompressStart(0, 0, "DeCompress_ZIP Start");

                    //개별 엔트리 받아서
                    ZipEntry zeEntry;
                    while ((zeEntry = Zipinput.GetNextEntry()) != null)
                    {

                        long nWorkBytes = 0;

                        long nZipLength;

                        try
                        {
                            nZipLength = Zipinput.Length;
                        }
                        catch (Exception ex)
                        {
                            nZipLength = 999999999999999;
                            OnCompressProgress(nWorkBytes, nZipLength, "DeCompress_ZIP Length No Read : " + ex.Message);

                        }

                        DirectoryInfo di;
                        string directoryName = Path.GetDirectoryName(zeEntry.Name);
                        string fileName = Path.GetFileName(zeEntry.Name);

                        // create directory
                        if (directoryName.Length > 0)
                        {
                            di = Directory.CreateDirectory(diDesc.FullName + Path.DirectorySeparatorChar.ToString() + directoryName);
                        }
                        else
                        {
                            di = new DirectoryInfo(diDesc.FullName);
                        }

                        if (fileName != String.Empty)
                        {
                            //string strDecompressedFilePathName = diDesc.FullName + @"\" + zeEntry.Name;
                            string strDecompressedFilePathName = di.FullName + Path.DirectorySeparatorChar.ToString() + fileName;
                            using (FileStream streamWriter = File.Create(strDecompressedFilePathName))
                            {
                                int nWhile = 0;
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = Zipinput.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                        nWorkBytes = nWorkBytes + size;

                                        nWhile++;
                                        if (nWhile % 10 == 0)
                                        {
                                            OnCompressProgress(nWorkBytes, nZipLength, "DeCompress_ZIP Working");

                                            //무한히 커지는 경우가 있어서 강제로 중지시키기 1000배 이상으로 압축 풀리면 이상하다고 가정.
                                            if ((nSource_FileLength * 1000) < (nTotalWorkBytes + nWorkBytes))
                                            {
                                                throw new Exception("DeCompress_ZIP Length Abnormal : SourceLength=" + nSource_FileLength.ToString() + ", Current DecompressingLength=" + (nTotalWorkBytes + nWorkBytes).ToString());
                                            }
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                lsDecompressedFilesPathName.Add(strDecompressedFilePathName);

                                nTotalWorkBytes = nTotalWorkBytes + nWorkBytes;
                            }
                        }

                    }


                    OnCompressEnd(nTotalWorkBytes, nTotalWorkBytes, "DeCompress_ZIP End");

                    Zipinput.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;

        }


        #region 이벤트

        /// <summary>
        /// 파일 압축 이벤트 델리게이트
        /// </summary>
        /// <param name="workBytes"></param>
        /// <param name="totalBytes"></param>
        /// <param name="message"></param>
        public delegate void FileCompressEvent(long workBytes, long totalBytes, string message);
        /// <summary>
        /// 파일 압축 이벤트
        /// </summary>
        public event FileCompressEvent fcEvent;
        /// <summary>
        /// 파일 압축 이벤트 발생
        /// </summary>
        /// <param name="workBytes"></param>
        /// <param name="totalBytes"></param>
        /// <param name="msg"></param>
        protected void OnCompressProgressEvent(long workBytes, long totalBytes, string msg)
        {
            if (fcEvent != null)
            {
                fcEvent(workBytes, totalBytes, msg);
            }
        }

        private void OnCompressStart(long workBytes, long totalBytes, string message)
        {
            OnCompressProgressEvent(workBytes, totalBytes, message);
        }
        private void OnCompressProgress(long workBytes, long totalBytes, string message)
        {
            OnCompressProgressEvent(workBytes, totalBytes, message);
        }
        private void OnCompressEnd(long workBytes, long totalBytes, string message)
        {
            OnCompressProgressEvent(workBytes, totalBytes, message);
        }

        #endregion
    }
}
