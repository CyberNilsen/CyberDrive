using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CyberDrive
{
    public class SecureDelete
    {
        public static void ShredFile(string filePath, int passes = 3)
        {
            var length = new FileInfo(filePath).Length;
            var buffer = new byte[length];
            using var rng = RandomNumberGenerator.Create();

            using var fs = new FileStream(filePath, FileMode.Open);
            for (int i = 0; i < passes; i++)
            {
                rng.GetBytes(buffer);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush(true);
            }
            fs.Close();
            File.Delete(filePath);
        }
    }

}
