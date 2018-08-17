using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Hashs
{
    public class SHA256 : Hash
    {
        public SHA256()
            : base(new SHA256CryptoServiceProvider())
        {

        }
    }
}
