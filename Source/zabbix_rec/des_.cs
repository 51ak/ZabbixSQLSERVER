using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace zabbix_rec
{
    class des_
    {
        String Key;
        String Vector;
        des_(String _Key, String _Vector)
        {
            Key = _Key;
            Vector = _Vector;
        }
        /*
    
           
 
            string YourInput = Console.ReadLine();
 
            //将字符串转换成字节
 
            byte[] YourInputStorage = System.Text.Encoding.UTF8.GetBytes(YourInput);
 
            Console.WriteLine("请输入你加密的密码:");
 
            string Password = Console.ReadLine();
 
            Console.WriteLine("请输入一个向量:");
 
            string Vector = Console.ReadLine();
 
            //将字符串转换成字节
 
            //byte[] password = System.Text.Encoding.UTF8.GetBytes(Password);
 
            Byte[] express=DESEncrypt(YourInputStorage,Password,Vector);
 
            Byte[] ciphertext = DESDecrypt(express, Password, Vector);
 
            Console.Write(System.Text.Encoding.ASCII.GetString(express, 0, express.Length));
 
            Console.Write("\n");
 
            Console.Write(System.Text.Encoding.ASCII.GetString(ciphertext, 0, ciphertext.Length));
         */



        #region DES加密解密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="Data">被加密的明文</param>
        /// <param name="Key">密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns>密文</returns>
        public  string  DESEncrypt(string oldstr)
        {
            Byte[] Data = System.Text.Encoding.UTF8.GetBytes(oldstr);
            Byte[] bKey = new Byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Cryptograph = null; // 加密后的密文
            DESCryptoServiceProvider EncryptProvider = new DESCryptoServiceProvider();
            EncryptProvider.Mode = CipherMode.CBC;
            EncryptProvider.Padding = PaddingMode.Zeros;
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,EncryptProvider.CreateEncryptor(bKey, bVector),CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(Data, 0, Data.Length);
                        Encryptor.FlushFinalBlock();
                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }
            return Cryptograph;
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="Data">被解密的密文</param>
        /// <param name="Key">密钥</param>
        /// <param name="Vector">向量</param>
        /// <returns>明文</returns>
        public  Byte[] DESDecrypt(Byte[] Data, String Key, String Vector)
        {
            Byte[] bKey = new Byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[8];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Original = null;
            DESCryptoServiceProvider CryptoProvider = new DESCryptoServiceProvider();
            CryptoProvider.Mode = CipherMode.CBC;
            CryptoProvider.Padding = PaddingMode.Zeros;
            try
            {
                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream(Data))
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory,CryptoProvider.CreateDecryptor(bKey, bVector),CryptoStreamMode.Read))
                    {
                        // 明文存储区
                        using (MemoryStream OriginalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                OriginalMemory.Write(Buffer, 0, readBytes);
                            }
                            Original = OriginalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                Original = null;
            }
            return Original;
        }
        #endregion
    }
}
