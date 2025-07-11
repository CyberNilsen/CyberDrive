using System.Security.Cryptography;
using System.IO;

namespace CyberDrive
{
    public class CryptoUtils
    {
        public static void EncryptFile(string inputPath, string outputPath, string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var key = new Rfc2898DeriveBytes(password, salt, 100_000).GetBytes(32);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var input = new FileStream(inputPath, FileMode.Open);
            using var output = new FileStream(outputPath, FileMode.Create);
            output.Write(salt, 0, salt.Length);
            output.Write(aes.IV, 0, aes.IV.Length);

            using var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
            input.CopyTo(cryptoStream);
        }

        public static void DecryptFile(string inputPath, string outputPath, string password)
        {
            using var input = new FileStream(inputPath, FileMode.Open);

            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            input.Read(salt, 0, 16);
            input.Read(iv, 0, 16);

            var key = new Rfc2898DeriveBytes(password, salt, 100_000).GetBytes(32);
            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var output = new FileStream(outputPath, FileMode.Create);
            using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
            cryptoStream.CopyTo(output);
        }
    }
}
