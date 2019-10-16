using System;

namespace iPhoneTools
{
    public static class WrappedKeyReader
    {
        public static WrappedKey Read(byte[] data)
        {
            WrappedKey result = default;

            if (data.Length == 44)
            {
                result = new WrappedKey
                {
                    Unknown = data.AsSpan(0, 4).ToArray(),
                    Key = data.AsSpan(4, 40).ToArray(),
                };
            }

            return result;
        }
    }
}
