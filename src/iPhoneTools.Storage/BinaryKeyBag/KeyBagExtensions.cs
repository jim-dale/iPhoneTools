using System;
using System.Buffers.Binary;
using System.IO;

namespace iPhoneTools
{
    public static class KeyBagExtensions
    {
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
