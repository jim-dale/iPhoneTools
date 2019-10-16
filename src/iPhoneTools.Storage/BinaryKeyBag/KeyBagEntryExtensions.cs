using System;
using System.Buffers.Binary;
using System.IO;
using RFC3394;

namespace iPhoneTools
{
    internal static class KeyBagEntryExtensions
    {
        internal static byte[] UnwrapClassKey(this KeyBagEntry item, byte[] kek)
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

        internal static void SetValue(this KeyBagEntry item, string blockIdentifier, ReadOnlySpan<byte> value)
        {
            switch (blockIdentifier)
            {
                case KeyBagConstants.UuidTag:
                    item.Uuid = KeyBagExtensions.ReadGuidBigEndian(value);
                    break;
                case KeyBagConstants.ClassTag:
                    item.ProtectionClass = (ProtectionClass)BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.WrapTag:
                    item.Wrap = (WrapTypes)BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.KeyTypeTag:
                    item.KeyType = (KeyType)BinaryPrimitives.ReadInt32BigEndian(value);
                    break;
                case KeyBagConstants.WrappedKeyTag:
                    item.Wpky = value.ToArray();
                    break;
                default:
                    throw new InvalidDataException($"Unexpected block identifier \"{blockIdentifier}\"");
            }
        }
    }
}
