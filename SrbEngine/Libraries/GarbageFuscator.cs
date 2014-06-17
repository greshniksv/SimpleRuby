using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Serialization;

namespace SrbEngine.Libraries
{
    public class GarbageFuscator
    {
        private readonly List<long> _usingPositions = new List<long>();
	    private string _charsForGenerate = "!@#$%^&*()_+`1234567890-=qwertyuiop[]\\asdfghjkl;" +
	                                      "'zxcv_bnm,./QWERTYUIOP{}|ASDFGHJKL:\"ZXCVBNM<>?";


        public string Crypt(string password, string data)
        {
            _usingPositions.Clear();
	        for (int i = 0; i < 6; i++) _charsForGenerate += data;
            var codedBytes = Encoding.UTF8.GetBytes(data);
            var gens = GenerateBlock(codedBytes.Count() * 10);
            var passHash = Encoding.UTF8.GetBytes(Hash.GetMd5Hash(password));
            var retData = "";

            var passPos = 0;
            var dataPos = 0;
            using (var ms = new MemoryStream(gens))
            {
                // write data length
				ms.Write(Int2bytes(codedBytes.Count()), 0, 3);

                while (dataPos < codedBytes.Count())
                {
					// find not using position
	                while (_usingPositions.Any(i => i == ms.Position))
	                {
						if (ms.Position>=ms.Length)
							ms.Seek(0, SeekOrigin.Begin);
						else
			                ms.Seek(1, SeekOrigin.Current);
	                }

					if (ms.Position >= ms.Length) ms.Seek(3, SeekOrigin.Begin);

                    _usingPositions.Add(ms.Position);
					ms.Write(new [] { codedBytes[dataPos++] }, 0, 1);
                    if (++passPos >= passHash.Length) passPos = 0;

	                if (ms.Position + passHash[passPos] > ms.Length)
						ms.Seek(((ms.Position + passHash[passPos]) - ms.Length ), SeekOrigin.Begin);
					else
						ms.Seek(passHash[passPos], SeekOrigin.Current);
                }

	            var getBytes = new byte [(int)ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(getBytes, 0, (int)ms.Length);
                retData = Encoding.UTF8.GetString(getBytes);
            }

            return retData;
        }



		public string Decrypt(string password, string data)
		{
			_usingPositions.Clear();
			var codedBytes = Encoding.UTF8.GetBytes(data);
			//var gens = GenerateBlock(codedBytes.Count() * 30);
			var passHash = Encoding.UTF8.GetBytes(Hash.GetMd5Hash(password));
			var retData = "";

			var passPos = 0;
			var dataPos = 0;
			using (var ms = new MemoryStream(codedBytes))
			{
				var buf = new byte[3];
				// write data length
				ms.Read(buf, 0, 3);
				int dataLength = Byte2int(buf);
				var wordbuf = new byte[dataLength];
				buf = new byte[1];

				while (dataPos < dataLength)
				{

					// find not using position
					while (_usingPositions.Any(i => i == ms.Position))
					{
						if (ms.Position >= ms.Length)
							ms.Seek(0, SeekOrigin.Begin);
						else
							ms.Seek(1, SeekOrigin.Current);
					}

					if (ms.Position >= ms.Length) ms.Seek(3, SeekOrigin.Begin);


					_usingPositions.Add(ms.Position);
					ms.Read(buf, 0, 1);
					wordbuf[dataPos++] = buf[0];
					if (++passPos >= passHash.Length) passPos = 0;

					if (ms.Position + passHash[passPos] > ms.Length)
						ms.Seek(((ms.Position + passHash[passPos]) - ms.Length), SeekOrigin.Begin);
					else
						ms.Seek(passHash[passPos], SeekOrigin.Current);
				}

				retData = Encoding.UTF8.GetString(wordbuf);
			}

			return retData;
		}

        private byte[] GenerateBlock(int lenght)
        {
	        var bytemas = new byte[lenght];
            var random = new Random();
	        for (int i = 0; i < lenght; i++)
				bytemas[i] = Convert.ToByte( _charsForGenerate[random.Next(1, _charsForGenerate.Length)]);
			return bytemas;
        }


        private byte[] Int2bytes(int i)
        {
            var b = new byte[3];
            int divider = 0;
            while (i/++divider > 256) { }
            int dividerRest = (int)Math.Round((decimal)(i / divider));
            int rest = i - (divider*dividerRest);
            if (divider>=256) throw new Exception("To big number !!");

			b[0] = (byte)rest;
            b[1] = (byte)dividerRest;
            b[2] = (byte)divider;

			return b;
        }

		private int Byte2int(byte[] i)
		{
			return i[0]+(i[1]*i[2]);
		}


    }
}
