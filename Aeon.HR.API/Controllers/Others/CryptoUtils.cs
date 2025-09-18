using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;

namespace Aeon.HR.API.Controllers.Others
{
    public class CryptoUtils
    {
        private static RSA CreateCipherDecrypt(RSAParameters privateKeyParams)
        {
            RSA rsa = RSA.Create();
            rsa.ImportParameters(privateKeyParams);
            return rsa;
        }

        private static RSA CreateCipherEncrypt(RSAParameters publicKeyParams)
        {
            RSA rsa = RSA.Create();
            rsa.ImportParameters(publicKeyParams);
            return rsa;
        }

        public static XmlDocument BuildDocument(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        public static RSAParameters GetPublicKeyFromXML(string xml)
        {
            XmlDocument doc = BuildDocument(xml);
            RSAParameters parameters = new RSAParameters
            {
                Modulus = Convert.FromBase64String(doc.SelectSingleNode("//Modulus").InnerText),
                Exponent = Convert.FromBase64String(doc.SelectSingleNode("//Exponent").InnerText)
            };
            return parameters;
        }

        public static byte[] GenerateKey()
        {
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                tripleDES.GenerateKey();
                return tripleDES.Key;
            }
        }

        public static string DecryptTripleDes(string encryptedData, byte[] randomKey)
        {
            if (randomKey.Length != 16 && randomKey.Length != 24)
            {
                throw new ArgumentException("Key phải có độ dài 16 hoặc 24 bytes.");
            }

            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                tripleDES.Key = randomKey;
                tripleDES.Mode = CipherMode.ECB;  // Hoặc đổi sang CBC nếu cần IV
                tripleDES.Padding = PaddingMode.PKCS7;
                byte[] encryptedBytes = Convert.FromBase64String(encryptedData);
                using (ICryptoTransform decryptor = tripleDES.CreateDecryptor())
                {
                    byte[] originalData = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(originalData);
                }
            }
        }

        public static string EncryptTripleDes(string originalData, byte[] key)
        {
            using (TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider())
            {
                tripleDES.Key = key;
                tripleDES.Mode = CipherMode.ECB;
                tripleDES.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform encryptor = tripleDES.CreateEncryptor())
                {
                    byte[] inputBuffer = Encoding.UTF8.GetBytes(originalData);
                    byte[] result = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);
                    return Convert.ToBase64String(result);
                }
            }
        }

        public static byte[] DecryptRSAToByte(string encryptedKey, RSAParameters privateKeyParams)
        {
            using (RSA rsa = CreateCipherDecrypt(privateKeyParams))
            {
                return rsa.Decrypt(Convert.FromBase64String(encryptedKey), RSAEncryptionPadding.Pkcs1);
            }
        }

        public static string EncryptRSA(byte[] randomKey, RSAParameters publicKeyParams)
        {
            using (RSA rsa = CreateCipherEncrypt(publicKeyParams))
            {
                byte[] encryptedKey = rsa.Encrypt(randomKey, RSAEncryptionPadding.Pkcs1);
                return Convert.ToBase64String(encryptedKey);
            }
        }

        public static bool VerifyRSA(string signedData, string signature, RSAParameters publicKeyParams)
        {
            using (RSA rsa = CreateCipherEncrypt(publicKeyParams))
            {
                return rsa.VerifyData(Encoding.UTF8.GetBytes(signedData), Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static string SignRSA(string signatureData, RSAParameters privateKeyParams)
        {
            using (RSA rsa = CreateCipherDecrypt(privateKeyParams))
            {
                byte[] signature = rsa.SignData(Encoding.UTF8.GetBytes(signatureData), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return Convert.ToBase64String(signature);
            }
        }
    }
}