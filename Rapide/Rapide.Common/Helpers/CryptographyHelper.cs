using System.Security.Cryptography;
using System.Text;

namespace Rapide.Common.Helpers
{
    public static class CryptographyHelper
    {
        private static byte[] key = { };
        private static readonly byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        #region Decryption Level
        /// <summary>
        /// Decrypt Querstring Value
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Decrypt(string input, string encryptionKey)
        {
            return CryptographyHelper.DecryptToLevel1(input, true, encryptionKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <returns></returns>
        private static string DecryptToLevel1(string toDecrypt, bool useHashing, string encryptionKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = encryptionKey;

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return CryptographyHelper.DecryptToLevel2(UTF8Encoding.UTF8.GetString(resultArray), encryptionKey);
        }

        private static string DecryptToLevel2(string toDecrypt, string encryptionKey)
        {
            byte[] inputByteArray;

            try
            {
                key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(toDecrypt);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion Decryption Level

        #region Encryption Level
        /// <summary>
        /// Encrypt QueryString Value
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string Encrypt(string intput, string encryptionKey)
        {
            return CryptographyHelper.EncryptionToLevel1(intput, encryptionKey);
        }

        /// <summary>
        /// Encryption Level 2
        /// </summary>
        /// <param name="encryptionLevel2"></param>
        /// <returns></returns>
        private static string EncryptionToLevel1(string encryptionLevel1, string encryptionKey)
        {
            // Encrypt Level 2 here!
            try
            {
                key = Encoding.UTF8.GetBytes(encryptionKey.Substring(0, 8));
                var des = new DESCryptoServiceProvider();
                Byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptionLevel1);
                var ms = new MemoryStream();
                var cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return CryptographyHelper.EncryptionToLevel2(Convert.ToBase64String(ms.ToArray()), true, encryptionKey);
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="encryptionLevel2"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        private static string EncryptionToLevel2(string encryptionLevel2, bool useHashing, string encryptionKey)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(encryptionLevel2);

            //System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = encryptionKey;

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock
                    (toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        public static string GenerateSalt()
        {
            byte[] data = new byte[0x10];
            new RNGCryptoServiceProvider().GetBytes(data);

            return Convert.ToBase64String(data);
        }

        public static string GetEncryptionKey()
        {
            return "SIXRAM_TECHNOLOGIES_2025";
        }
    }
}