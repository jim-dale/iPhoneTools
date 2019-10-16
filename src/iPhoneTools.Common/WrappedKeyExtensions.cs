using System.Collections.Generic;
using RFC3394;

namespace iPhoneTools
{
    public static class WrappedKeyExtensions
    {
        public static byte[] UnwrapKey(this WrappedKey item, IReadOnlyDictionary<ProtectionClass, byte[]> classKeys)
        {
            var protectionClass = (ProtectionClass)item.Unknown[0];

            return item.UnwrapKey(protectionClass, classKeys);
        }

        public static byte[] UnwrapKey(this WrappedKey item, ProtectionClass protectionClass, IReadOnlyDictionary<ProtectionClass, byte[]> classKeys)
        {
            var kek = classKeys[protectionClass];

            return KeyWrapAlgorithm.UnwrapKey(kek, item.Key);
        }
    }
}
