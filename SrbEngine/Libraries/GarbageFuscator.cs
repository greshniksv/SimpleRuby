using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SrbEngine.Libraries
{
    public class GarbageFuscator
    {
        // 128 chars
        private const string Data = "1234567890-=\\!@#$%^&*()_+|qwertyuiop[]{" +
                                     "}QWERTYUIOPasdfghjkl;'ASDFGHJKL:\"zxcvbnm,./ZXCVBNM<>?`~"+
                                     "йцукёнгшщзхъЙЦУКЕНГШЩЗХЪфывапролдж";

        private readonly List<long> _usingPositions = new List<long>(); 


        public string Crypt(string password, string data)
        {
            _usingPositions.Clear();
            var codedBytes = Encoding.UTF8.GetBytes(data);
            var gens = GenerateBlock(codedBytes.Count() * 30);
            var passHash = Encoding.UTF8.GetBytes(Hash.GetMd5Hash(password));
            var retData = "";

            var passPos = 0;
            var dataPos = 0;
            using (var ms = new MemoryStream(gens))
            {
                // write data length
                ms.Write(Int2(codedBytes.Count()),0,3);

                while (dataPos < codedBytes.Count())
                {
                    if (ms.Position + passHash[passPos] > ms.Length)
                    {
                        var p = (ms.Length - (ms.Position + passHash[passPos]));
                        while (_usingPositions.Any(i => i == p || i == p + 1))
                        {
                            p++;
                        }
                        ms.Seek(p, SeekOrigin.Begin);
                    }
                    else
                    {
                        var p = ms.Position + passHash[passPos];
                        while (_usingPositions.Any(i => i == p || i == p + 1))
                        {
                            p++;
                        }

                        ms.Seek(p, SeekOrigin.Begin);
                    }

                    _usingPositions.Add(ms.Position);
                    _usingPositions.Add(ms.Position+1);
                    ms.Write(Byte2(codedBytes[dataPos++]),0,2);
                    if (passPos++ >= passHash.Length) passPos = 0;
                }

                byte[] getBytes = new byte[1];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(getBytes, 0, (int)ms.Length);
                retData = Encoding.UTF8.GetString(getBytes);
            }

            return retData;
        }

        private byte[] GenerateBlock(int lenght)
        {
            var stringBuilder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < lenght; i++)
                stringBuilder.Append(Data[random.Next(0, 127)]);
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        private byte[] Byte2(byte c)
        {
            var b = new byte[2];
            if (c < 128)
            {
                b[0] = Convert.ToByte(Data[c]);
                b[1] = Convert.ToByte(Data[0]);
            }
            else
            {
                b[0] = Convert.ToByte(Data[c - 128]);
                b[1] = Convert.ToByte(Data[(c - (c - 128))]);
            }
            return b;
        }


        private byte[] Int2(int i)
        {
            var b = new byte[3];
            int divider = 0;
            while (i/++divider > 128) { }
            int dividerRest = (int)Math.Round((decimal)(i / divider));
            int rest = i - (divider*dividerRest);
            if (divider>=128) throw new Exception("To big number !!");

            b[0] = Convert.ToByte(Data[rest]);
            b[1] = Convert.ToByte(Data[dividerRest]);
            b[2] = Convert.ToByte(Data[divider]);

            return b;
        }


    }
}
