
using System;
using System.Linq;
using System.Security.Cryptography;

namespace BackendProject.Helpers
{
    public class PasswordHelper
    {
        public string CreateHashedPassword(string password)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, 20);
            byte[] salt = deriveBytes.Salt;
            byte[] key = deriveBytes.GetBytes(32);  // derive a 20-byte key

            return string.Format("{0}.{1}", Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public bool CompareHashedPassword(string saltKey, string rawPassword)
        {
            byte[] salt = Convert.FromBase64String(saltKey.Split('.').FirstOrDefault());
            byte[] key = Convert.FromBase64String(saltKey.Split('.').LastOrDefault());
            using var deriveBytes = new Rfc2898DeriveBytes(rawPassword, salt);
            byte[] newKey = deriveBytes.GetBytes(32);  // derive a 20-byte key
            return newKey.SequenceEqual(key);
        }
    }
}