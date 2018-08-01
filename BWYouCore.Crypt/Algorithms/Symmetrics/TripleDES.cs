using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Symmetrics
{
    public class TripleDES : Symmetric
    {
        public TripleDES(byte[] key, byte[] iv)
            : base(new TripleDESCryptoServiceProvider(), key, iv)
        {

        }
        public TripleDES(string base64Key, string base64Iv)
            : base(new TripleDESCryptoServiceProvider(), base64Key, base64Iv)
        {

        }
        public TripleDES(out byte[] key, out byte[] iv)
            : base(new TripleDESCryptoServiceProvider(), out key, out iv)
        {

        }
        public TripleDES(out string base64Key, out string base64Iv)
            : base(new TripleDESCryptoServiceProvider(), out base64Key, out base64Iv)
        {

        }
    }
}
