using System;
using System.IO;
using System.Security.Cryptography;

namespace CyberDrive
{
    public class SecureDelete
    {
        public static void ShredFile(string filePath, int passes = 3)
        {
            if (!File.Exists(filePath))
                return;

            var fileInfo = new FileInfo(filePath);
            var length = fileInfo.Length;

            if (length == 0)
            {
                File.Delete(filePath);
                return;
            }

            using var rng = RandomNumberGenerator.Create();
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write);

            for (int pass = 0; pass < passes; pass++)
            {
                fs.Seek(0, SeekOrigin.Begin);

                const int chunkSize = 1024 * 1024;
                var buffer = new byte[Math.Min(chunkSize, length)];

                long bytesRemaining = length;
                while (bytesRemaining > 0)
                {
                    int bytesToWrite = (int)Math.Min(buffer.Length, bytesRemaining);
                    rng.GetBytes(buffer, 0, bytesToWrite);
                    fs.Write(buffer, 0, bytesToWrite);
                    bytesRemaining -= bytesToWrite;
                }

                fs.Flush(true);
            }

            fs.Close();
            File.Delete(filePath);
        }
    }
}