using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace iPhoneTools
{
    public static class KeyBagExtensions
    {
        public static IReadOnlyDictionary<ProtectionClass, byte[]> UnwrapClassKeys_v1(this KeyBag item, string password)
        {
            Dictionary<ProtectionClass, byte[]> result = default;

            using (var generator = new Rfc2898DeriveBytes(password, item.Salt, item.Iterations, HashAlgorithmName.SHA1))
            {
                var kek = generator.GetBytes(32);

                result = item.UnwrapClassKeys(kek);
            }

            return result;
        }

        public static IReadOnlyDictionary<ProtectionClass, byte[]> UnwrapClassKeys_v2(this KeyBag item, string password)
        {
            Dictionary<ProtectionClass, byte[]> result = default;

            if (item.DataProtection != null)
            {
                using (var gen1 = new Rfc2898DeriveBytes(password, item.DataProtection.Dpsl, item.DataProtection.Dpic, HashAlgorithmName.SHA256))
                {
                    var derivedPasscode = gen1.GetBytes(32);

                    using (var gen2 = new Rfc2898DeriveBytes(derivedPasscode, item.Salt, item.Iterations, HashAlgorithmName.SHA1))
                    {
                        var kek = gen2.GetBytes(32);

                        result = item.UnwrapClassKeys(kek);
                    }
                }
            }

            return result;
        }

        public static Dictionary<ProtectionClass, byte[]> UnwrapClassKeys(this KeyBag item, byte[] kek)
        {
            var result = new Dictionary<ProtectionClass, byte[]>();

            foreach (var entry in item.WrappedKeys)
            {
                var classKey = entry.UnwrapClassKey(kek);
                if (classKey != default)
                {
                    result.Add(entry.ProtectionClass, classKey);
                }
            }

            return result;
        }

        public static void SetValue(this KeyBag item, string blockIdentifier, ReadOnlySpan<byte> value)
        {
            switch (blockIdentifier)
            {
                case KeyBagConstants.VersionTag:
                    item.Version = BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.TypeTag:
                    item.KeyBagType = (KeyBagType)BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.UuidTag:
                    item.Uuid = ReadGuidBigEndian(value);
                    break;
                case KeyBagConstants.HmckTag:
                    item.HMCK = value.ToArray();
                    break;
                case KeyBagConstants.WrapTag:
                    item.Wrap = BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.SaltTag:
                    item.Salt = value.ToArray();
                    break;
                case KeyBagConstants.IterTag:
                    item.Iterations = BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.DpwtTag:
                case KeyBagConstants.DpicTag:
                case KeyBagConstants.DpslTag:
                    SetDataProtectionValue(item, blockIdentifier, value);
                    break;
                default:
                    throw new InvalidDataException($"Unexpected block identifier \"{blockIdentifier}\"");
            }
        }

        private static void SetDataProtectionValue(KeyBag item, string blockIdentifier, ReadOnlySpan<byte> value)
        {
            if (item.DataProtection == null)
            {
                item.DataProtection = new DataProtectionKeyData();
            }
            switch (blockIdentifier)
            {
                case KeyBagConstants.DpwtTag:
                    item.DataProtection.Dpwt = BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.DpicTag:
                    item.DataProtection.Dpic = BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.DpslTag:
                    item.DataProtection.Dpsl = value.ToArray();
                    break;
                default:
                    throw new InvalidDataException($"Unexpected block identifier \"{blockIdentifier}\"");
            }
        }

        public static Guid ReadGuidBigEndian(ReadOnlySpan<byte> value)
        {
            var a = BinaryPrimitives.ReadUInt32BigEndian(value.Slice(0, 4));
            var b = BinaryPrimitives.ReadUInt16BigEndian(value.Slice(4, 2));
            var c = BinaryPrimitives.ReadUInt16BigEndian(value.Slice(6, 2));

            return new Guid(a, b, c, value[8], value[9], value[10], value[11], value[12], value[13], value[14], value[15]);
        }
    }
}
