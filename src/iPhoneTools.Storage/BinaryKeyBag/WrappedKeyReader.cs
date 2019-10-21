using System;

namespace iPhoneTools
{
    public static class WrappedKeyReader
    {
        public static WrappedKey Read(byte[] wrappedKeyData)
        {
            if (wrappedKeyData is null)
            {
                throw new ArgumentNullException(nameof(wrappedKeyData));
            }

            WrappedKey result = default;

            if (wrappedKeyData.Length == 44)
            {
                result = new WrappedKey
                {
                    Unknown = wrappedKeyData.AsSpan(0, 4).ToArray(),
                    Key = wrappedKeyData.AsSpan(4, 40).ToArray(),
                };
            }

            return result;
        }
    }
}
