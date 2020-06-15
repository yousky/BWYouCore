using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Cloud.Storage
{
    /// <summary>
    /// Cloud Storage manipulation
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Upload To Storage
        /// </summary>
        /// <param name="inputStream">inputStream</param>
        /// <param name="sourcefilename">Source File Name</param>
        /// <param name="containerName">container Name</param>
        /// <param name="destpath">Storage Destination path</param>
        /// <param name="useUUIDName">true : UUIDName use, false : sourcefilename use</param>
        /// <param name="overwrite">overwrite true, false</param>
        /// <param name="useSequencedName">overwrite false and same file exist then filename[1], filename[2], ... use</param>
        /// <returns>Saved File's Storage Uri</returns>
        Task<UploadedInfo> UploadAsync(Stream inputStream, string sourcefilename, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true);

        /// <summary>
        /// Upload To Storage
        /// </summary>
        /// <param name="sourcefilepathname">upload source File Full Path</param>
        /// <param name="containerName">container Name</param>
        /// <param name="destpath">Storage Destination path</param>
        /// <param name="useUUIDName">true : UUIDName use, false : sourcefilename use</param>
        /// <param name="overwrite">overwrite true, false</param>
        /// <param name="useSequencedName">overwrite false and same file exist then filename[1], filename[2], ... use</param>
        /// <returns>Saved File's Storage Uri</returns>
        Task<UploadedInfo> UploadAsync(string sourcefilepathname, string containerName, string destpath = "", bool useUUIDName = true, bool overwrite = false, bool useSequencedName = true);

        /// <summary>
        /// Download To File
        /// </summary>
        /// <param name="sourceUri">Source URI</param>
        /// <param name="destfilename">Destination File Full Path</param>
        /// <param name="overwrite">overwrite true, false</param>
        /// <param name="useSequencedName">overwrite false and same file exist then filename[1], filename[2], ... use</param>
        /// <returns>Downloaded File's FullName</returns>
        Task<string> DownloadAsync(Uri sourceUri, string destfilename, bool overwrite = false, bool useSequencedName = true);
        /// <summary>
        /// Download To Stream
        /// </summary>
        /// <param name="sourceUri">Source URI</param>
        /// <param name="deststream">Destination Stream</param>
        /// <returns>Downloaded File's FullName</returns>
        Task<string> DownloadAsync(Uri sourceUri, Stream deststream);
        /// <summary>
        /// Delete source
        /// </summary>
        /// <param name="sourceUri">Source URI</param>
        Task DeleteAsync(Uri sourceUri);
    }

    public class UploadedInfo
    {
        public string AbsoluteUri { get; set; }
        public long Length { get; set; }
    }
}
