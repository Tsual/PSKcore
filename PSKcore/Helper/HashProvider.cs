using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PSK.Helper
{
    public class HashProvider
    {
        HashAlgorithm _Hash = null;

        public enum HashAlgorithms { MD5, SHA512 ,}

        public HashProvider()
        {
            _Hash = SHA256.Create(); 
        }
        public HashProvider(string name)
        {
            _Hash = MD5.Create();
        }


        public string Hash(string str)
        {
            return gets(_Hash.ComputeHash(getb(str)));
        }

        public byte[] Hashbytes(string str)
        {
            return _Hash.ComputeHash(getb(str));
        }

        static string gets(byte[] ba)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ba.Length; i++)
            {
                sb.Append(Convert.ToChar(ba[i]));
            }
            return sb.ToString();
        }
        static byte[] getb(string str)
        {
            List<byte> lb = new List<byte>();
            for (int i = 0; i < str.Length; i++)
            {
                lb.Add(Convert.ToByte(str[i]));
            }
            return lb.ToArray();
        }
    }
}
