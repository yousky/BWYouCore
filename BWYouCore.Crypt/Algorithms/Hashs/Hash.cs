using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Hashs
{
    public class Hash : IDisposable
    {
        HashAlgorithm hash;

        protected Hash(HashAlgorithm hashAlgorithm)
        {
            hash = hashAlgorithm;
        }

        public byte[] ComputeHash(byte[] srcData)
        {
            return hash.ComputeHash(srcData);
        }
        public byte[] ComputeHashFromUTF8String(string srcUTF8String)
        {
            byte[] srcData = Encoding.UTF8.GetBytes(srcUTF8String);
            return ComputeHash(srcData);
        }
        public string ComputeHashToBase64String(byte[] srcData)
        {
            return Convert.ToBase64String(ComputeHash(srcData));
        }
        public string ComputeHashToHexString(byte[] srcData)
        {
            return BitConverter.ToString(ComputeHash(srcData));
        }
        public string ComputeHashFromUTF8StringToBase64String(string srcUTF8String)
        {
            return Convert.ToBase64String(ComputeHashFromUTF8String(srcUTF8String));
        }
        public string ComputeHashFromUTF8StringToHexString(string srcUTF8String)
        {
            return BitConverter.ToString(ComputeHashFromUTF8String(srcUTF8String));
        }

        public void Dispose()
        {
            if (hash != null)
            {
                hash.Dispose();
            }
        }
    }
}
