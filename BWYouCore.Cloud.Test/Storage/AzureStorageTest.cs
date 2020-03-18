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
    class AzureStorageTest
    {
        //[Test]
        public async Task UploadAndDownloadAsync()
        {
            // 정렬
            string connectionString = @"set required";  //set required
            string container = @"test";
            string destpath = @"Dest";
            string srcpathname = Path.Combine(Path.GetDirectoryName(typeof(FileStorageTest).Assembly.Location), @"Storage\Sample\Dest\test.js");
            IStorage storage = new AzureStorage(connectionString);

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
            string uri = await storage.UploadAsync(srcpathname, container, destpath, true, false);

            // 어설션
            Assert.IsInstanceOf(typeof(DuplicateFileException), ex);
            Assert.IsTrue(uri.Contains(Path.Combine(container, destpath).Replace(@"\", @"/")));
        }
    }
}
