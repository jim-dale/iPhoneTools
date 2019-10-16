using System;

namespace RFC3394.UnitTests
{
    internal static class Helpers
    {
        public static string ByteArrayToHexString(byte[] value)
        {
            var result = BitConverter.ToString(value);
            return result.Replace("-", string.Empty);
        }

        public static byte[] HexStringToByteArray(string value)
        {
            if (value.Length % 2 != 0)
                throw new ArgumentException("The binary key cannot have an odd number of digits");

            var result = new byte[value.Length >> 1];

            int j = 0;
            for (int i = 0; i < result.Length; ++i)
            {
                var h = GetHexValue(value[j++]);
                var l = GetHexValue(value[j++]);

                result[i] = (byte)(h << 4 | l);
            }

            return result;
        }

        public static int GetHexValue(char ch)
        {
            switch (ch)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'a':
                case 'A': return 10;
                case 'b':
                case 'B': return 11;
                case 'c':
                case 'C': return 12;
                case 'd':
                case 'D': return 13;
                case 'e':
                case 'E': return 14;
                case 'f':
                case 'F': return 15;
                default:
                    throw new ArgumentOutOfRangeException("Unrecognized hex character " + ch);
            }
        }
    }
}
