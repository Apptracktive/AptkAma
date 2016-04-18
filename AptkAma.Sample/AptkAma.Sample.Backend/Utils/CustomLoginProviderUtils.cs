using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace AptkAma.Sample.Backend.Utils
{
    public class CustomLoginProviderUtils
    {
        public static byte[] Hash(string plaintext, byte[] salt)
        {
            var hashFunc = new SHA512Cng();
            var plainBytes = System.Text.Encoding.ASCII.GetBytes(plaintext);
            var toHash = new byte[plainBytes.Length + salt.Length];
            plainBytes.CopyTo(toHash, 0);
            salt.CopyTo(toHash, plainBytes.Length);
            return hashFunc.ComputeHash(toHash);
        }

        public static byte[] GenerateSalt()
        {
            var rng = new RNGCryptoServiceProvider();
            var salt = new byte[256];
            rng.GetBytes(salt);
            return salt;
        }

        public static bool SlowEquals(byte[] a, byte[] b)
        {
            var diff = a.Length ^ b.Length;
            for (var i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}