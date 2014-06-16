using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SrbEngine.Libraries
{
    public static class Hash
    {
        public static string GetMd5Hash(string input)
        {
            var sBuilder = new StringBuilder();
            using ( MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(i.ToString("x2"));
            }
            return sBuilder.ToString();
        }


    }
}
