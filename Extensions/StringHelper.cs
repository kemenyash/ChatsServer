using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static class StringHelper
    {
        public static string GetRandomString()
        {
            Random random = new Random();
            const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 40)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static string ComputeHash(string data)
        {
            if (String.IsNullOrEmpty(data))
                return String.Empty;

            using (var sha = new SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(data);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
