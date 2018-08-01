using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Hashs
{
    public class SHA512 : Hash
    {
        public SHA512()
            : base(new SHA512CryptoServiceProvider())
        {

        }
    }
}
