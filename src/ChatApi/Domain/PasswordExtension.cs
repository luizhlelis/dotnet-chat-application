using System;
using System.Security.Cryptography;
using System.Text;

namespace ChatApi.Domain
{
    public static class PasswordExtension
    {
        public static string GetHashSha256(this string password)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(password);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += string.Format("{0:x2}", x);
            }
            return hashString;
        }
    }
}
