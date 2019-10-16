using System.IO;
using System.Security.Cryptography;

namespace iPhoneTools
{
    public static partial class Encryption
    {
        private static readonly byte[] DefaultIV = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        public static void DecryptFile(string inputPath, string outputPath, byte[] key, bool overwrite)
        {
            var fileMode = (overwrite) ? FileMode.Create : FileMode.CreateNew;

            using (var alg = new AesManaged())
            {
                alg.Key = key;
                alg.IV = DefaultIV;
                alg.Mode = CipherMode.CBC;

                var decryptor = alg.CreateDecryptor();

                using (var inStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var outStream = new FileStream(outputPath, fileMode))
                    {
                        using (var cryptoStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                        {
                            cryptoStream.CopyTo(outStream);
                        }
                    }
                }
            }
        }
    }
}
