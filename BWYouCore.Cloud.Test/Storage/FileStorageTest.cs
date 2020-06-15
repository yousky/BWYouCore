using BWYouCore.Cloud.Exceptions;
using BWYouCore.Cloud.Storage;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Cloud.Test.Storage
{
    [TestFixture]
    class FileStorageTest
    {
        [Test]
        public async Task UploadAndDownloadAsync()
        {
            // 정렬
            string publicRootUrl = @"https://test.doma.in/Storage";
            string rootPath = Path.Combine(Path.GetDirectoryName(typeof(FileStorageTest).Assembly.Location), @"Storage");
            string container = @"Sample";
            string destpath = @"Dest";
            string srcpathname = Path.Combine(Path.GetDirectoryName(typeof(FileStorageTest).Assembly.Location), @"Storage\Sample\Dest\test.js");

            FileInfo fi = new FileInfo(srcpathname);

            IStorage storage = new FileStorage(rootPath, publicRootUrl);

            // 동작
            Exception ex = null;
            try
            {
                await storage.UploadAsync(srcpathname, container, destpath, false, false, false);
            }
            catch (Exception e)
            {
                ex = e;
            }
            var uploadedInfo = await storage.UploadAsync(srcpathname, container, destpath, true, false);
            string uri = uploadedInfo.AbsoluteUri;

            // 어설션
            Assert.IsInstanceOf(typeof(DuplicateFileException), ex);
            Assert.IsTrue(uri.StartsWith(publicRootUrl) && uri.Contains(Path.Combine(container, destpath).Replace(@"\", @"/")));
            Assert.AreEqual(fi.Length, uploadedInfo.Length);
        }

        [Test]
        public async Task ShouldEqualUriWhenFileDownload()
        {
            // 정렬
            string publicRootUrl = @"https://test.doma.in/Storage";
            string rootPath = Path.Combine(Path.GetDirectoryName(typeof(FileStorageTest).Assembly.Location), @"Storage");
            string uri = @"https://test.doma.in/Storage/Sample/Dest/test.js";
            string destpath = Path.Combine(rootPath, "Download");
            string destfilename = @"test.js";
            IStorage storage = new FileStorage(rootPath, publicRootUrl);

            // 동작
            Exception ex = null;
            try
            {
                await storage.DownloadAsync(new Uri(uri), Path.Combine(destpath, destfilename), false, false);
                await storage.DownloadAsync(new Uri(uri), Path.Combine(destpath, destfilename), false, false);
            }
            catch (Exception e)
            {
                ex = e;
            }
            string filepath1 = await storage.DownloadAsync(new Uri(uri), Path.Combine(destpath, destfilename), true, true);
            string filepath2 = await storage.DownloadAsync(new Uri(uri), Path.Combine(destpath, destfilename), false, true);

            // 어설션
            Assert.IsInstanceOf(typeof(DuplicateFileException), ex);
            Assert.IsTrue(filepath1 == Path.Combine(destpath, destfilename));
            Assert.IsTrue(filepath2.StartsWith(destpath) && filepath2 != Path.Combine(destpath, destfilename));
        }
    }
}
