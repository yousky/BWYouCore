using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Symmetrics
{
    public class Aes : Symmetric
    {
        public Aes(byte[] key, byte[] iv)
            : base(new AesCryptoServiceProvider(), key, iv)
        {

        }
        public Aes(string base64Key, string base64Iv)
            : base(new AesCryptoServiceProvider(), base64Key, base64Iv)
        {

        }
        public Aes(out byte[] key, out byte[] iv)
            : base(new AesCryptoServiceProvider(), out key, out iv)
        {

        }
        public Aes(out string base64Key, out string base64Iv)
            : base(new AesCryptoServiceProvider(), out base64Key, out base64Iv)
        {

        }
    }
}
