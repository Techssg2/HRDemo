using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Aeon.HR.Infrastructure.Utilities
{
    public class Encryptor
    {
        public static string SHA256encrypt(string phrase)
        {
            UTF8Encoding encoder = new UTF8Encoding();
            SHA256Managed sha256hasher = new SHA256Managed();
            byte[] hashedDataBytes = sha256hasher.ComputeHash(encoder.GetBytes(phrase));
            return HexStringFromBytes(hashedDataBytes);
        }

        /// <summary>
        /// Generate sha1 hash with salt
        /// </summary>
        /// <param name="source"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateSaltedSHA1(string source, string salt = "")
        {
            HashAlgorithm algorithm = new SHA1Managed();
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var sourceBytes = Encoding.ASCII.GetBytes(source);
            var sourceWithSaltBytes = AppendByteArrays(saltBytes, sourceBytes);

            var sha1Bytes = algorithm.ComputeHash(sourceWithSaltBytes);
            return HexStringFromBytes(sha1Bytes);
        }

        private static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        private static byte[] AppendByteArrays(byte[] byteArrayA, byte[] byteArrayB)
        {
            var byteArrayResult =
                    new byte[byteArrayA.Length + byteArrayB.Length];

            for (var i = 0; i < byteArrayA.Length; i++)
                byteArrayResult[i] = byteArrayA[i];
            for (var i = 0; i < byteArrayB.Length; i++)
                byteArrayResult[byteArrayA.Length + i] = byteArrayB[i];

            return byteArrayResult;
        }
    }
}
