using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System;
using RFC3394;

namespace iPhoneTools
{
    public class KeyStore
    {
        private static readonly byte[] DefaultFileEncryptionIV = new byte[]
        {
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private readonly ILogger<MessageCommand> _logger;
        private WrappedKey _wrappedManifestKey;
        private IReadOnlyDictionary<ProtectionClass, byte[]> _classKeys;

        public KeyStore(ILogger<MessageCommand> logger)
        {
            _logger = logger;
        }

        public void UnwrapClassKeysFromKeyBag(KeyBag keyBag, string password)
        {
            if (keyBag is null)
            {
                throw new ArgumentNullException(nameof(keyBag));
            }

            _logger.LogInformation("Unwrapping class keys");

            var kek = (keyBag.DataProtection is null)
                ? DeriveKeyEncryptionKey_v1(keyBag, password)
                : DeriveKeyEncryptionKey_v2(keyBag, password);

            _classKeys = UnwrapClassKeys(keyBag, kek);
        }

        public void SetManifestKey(byte[] wrappedKeyData)
        {
            if (wrappedKeyData is null)
            {
                throw new ArgumentNullException(nameof(wrappedKeyData));
            }

            _wrappedManifestKey = WrappedKeyReader.Read(wrappedKeyData);
        }

        public void DecryptManifestFile(string inputPath, string outputPath, bool overwrite)
        {
            var key = UnwrapKey(_wrappedManifestKey, _classKeys);

            DecryptFileCore(inputPath, outputPath, key, overwrite);
        }

        public void DecryptFile(string inputPath, string outputPath, WrappedKey wrappedKey, ProtectionClass protectionClass, bool overwrite)
        {
            var key = UnwrapKey(wrappedKey, _classKeys, protectionClass);

            DecryptFileCore(inputPath, outputPath, key, overwrite);
        }

        private byte[] DeriveKeyEncryptionKey_v1(KeyBag item, string password)
        {
            byte[] result = default;

            using (var generator = new Rfc2898DeriveBytes(password, item.Salt, item.Iterations, HashAlgorithmName.SHA1))
            {
                result = generator.GetBytes(32);
            }

            return result;
        }

        private byte[] DeriveKeyEncryptionKey_v2(KeyBag item, string password)
        {
            byte[] result = default;

            using (var gen1 = new Rfc2898DeriveBytes(password, item.DataProtection.Dpsl, item.DataProtection.Dpic, HashAlgorithmName.SHA256))
            {
                var derivedPasscode = gen1.GetBytes(32);

                using (var gen2 = new Rfc2898DeriveBytes(derivedPasscode, item.Salt, item.Iterations, HashAlgorithmName.SHA1))
                {
                    result = gen2.GetBytes(32);
                }
            }

            return result;
        }

        private static IReadOnlyDictionary<ProtectionClass, byte[]> UnwrapClassKeys(KeyBag item, byte[] kek)
        {
            var result = new Dictionary<ProtectionClass, byte[]>();

            foreach (var wrappedKey in item.WrappedKeys)
            {
                var key = UnwrapClassKey(wrappedKey, kek);
                if (key != default)
                {
                    result.Add(wrappedKey.ProtectionClass, key);
                }
            }

            return result;
        }

        private static byte[] UnwrapClassKey(KeyBagEntry item, byte[] kek)
        {
            byte[] result = default;

            if (item.Wpky != null)
            {
                if ((item.Wrap & WrapTypes.Passcode) == WrapTypes.Passcode)
                {
                    result = KeyWrapAlgorithm.UnwrapKey(kek, item.Wpky);
                }
            }

            return result;
        }

        public static byte[] UnwrapKey(WrappedKey item, IReadOnlyDictionary<ProtectionClass, byte[]> classKeys)
        {
            var protectionClass = (ProtectionClass)item.Unknown[0];

            return UnwrapKey(item, classKeys, protectionClass);
        }

        public static byte[] UnwrapKey(WrappedKey item, IReadOnlyDictionary<ProtectionClass, byte[]> classKeys, ProtectionClass protectionClass)
        {
            var kek = classKeys[protectionClass];

            return KeyWrapAlgorithm.UnwrapKey(kek, item.Key);
        }

        private void DecryptFileCore(string inputPath, string outputPath, byte[] key, bool overwrite)
        {
            var fileMode = (overwrite) ? FileMode.Create : FileMode.CreateNew;

            using (var alg = new AesManaged())
            {
                alg.Key = key;
                alg.IV = DefaultFileEncryptionIV;
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
