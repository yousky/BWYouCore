using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BWYouCore.Crypt.Algorithms.Symmetrics
{
    /// <summary>
    /// 대칭키 암호화
    /// </summary>
    public class Symmetric : IDisposable
    {
        SymmetricAlgorithm symmetric;

        protected Symmetric(SymmetricAlgorithm symmetricAlgorithm)
        {
            symmetric = symmetricAlgorithm;
        }
        protected Symmetric(SymmetricAlgorithm symmetricAlgorithm, byte[] key, byte[] iv)
            : this(symmetricAlgorithm)
        {
            symmetric.Key = key;
            symmetric.IV = iv;
        }
        protected Symmetric(SymmetricAlgorithm symmetricAlgorithm, string base64Key, string base64Iv)
            : this(symmetricAlgorithm, Convert.FromBase64String(base64Key), Convert.FromBase64String(base64Iv))
        {

        }
        protected Symmetric(SymmetricAlgorithm symmetricAlgorithm, out byte[] key, out byte[] iv)
            : this(symmetricAlgorithm)
        {
            symmetric.GenerateKey();
            symmetric.GenerateIV();

            key = symmetric.Key;
            iv = symmetric.IV;
        }
        protected Symmetric(SymmetricAlgorithm symmetricAlgorithm, out string base64Key, out string base64Iv)
            : this(symmetricAlgorithm)
        {
            symmetric.GenerateKey();
            symmetric.GenerateIV();

            base64Key = Convert.ToBase64String(symmetric.Key);
            base64Iv = Convert.ToBase64String(symmetric.IV);
        }


        public byte[] Encrypt(byte[] planData)
        {
            using (ICryptoTransform encryptor = symmetric.CreateEncryptor())
            {
                return encryptor.TransformFinalBlock(planData, 0, planData.Length);
            }
        }
        public byte[] EncryptFromUTF8String(string planUTF8String)
        {
            byte[] planData = Encoding.UTF8.GetBytes(planUTF8String);
            return Encrypt(planData);
        }
        public string EncryptToBase64String(byte[] planData)
        {
            return Convert.ToBase64String(Encrypt(planData));
        }
        public string EncryptFromUTF8StringToBase64String(string planUTF8String)
        {
            return Convert.ToBase64String(EncryptFromUTF8String(planUTF8String));
        }
        public byte[] Decrypt(byte[] encryptedData)
        {
            using (ICryptoTransform decryptor = symmetric.CreateDecryptor())
            {
                return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }
        }
        public string DecryptToUTF8String(byte[] encryptedData)
        {
            byte[] planData = Decrypt(encryptedData);
            return Encoding.UTF8.GetString(planData);
        }
        public byte[] DecryptFromBase64String(string encryptedBase64String)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedBase64String);
            return Decrypt(encryptedData);
        }
        public string DecryptFromBase64StringToUTF8String(string encryptedBase64String)
        {
            byte[] encryptedData = Convert.FromBase64String(encryptedBase64String);
            return DecryptToUTF8String(encryptedData);
        }

        public void Dispose()
        {
            if (symmetric != null)
            {
                symmetric.Dispose();
            }
        }
    }
}
