using System.Text;

namespace iPhoneTools
{
    public static partial class CommonHelpers
    {
        public static string ByteArrayToDebugString(byte[] data)
        {
            var builder = new StringBuilder();

            ByteArrayToDebugString(data, 0, builder);

            return builder.ToString();
        }

        private static void ByteArrayToDebugString(byte[] data, int indent, StringBuilder target)
        {
            ByteArrayToDebugString(data, 0, data.Length, indent, target);
        }

        private static void ByteArrayToDebugString(byte[] value, int offset, int length, int indent, StringBuilder target)
        {
            int outputLength = ((length + 15) / 16) * 16;      // round up

            var chars = new StringBuilder();

            for (int i = 0; i < outputLength; i++)
            {
                if (i % 16 == 0)
                {
                    if (i > 0)
                    {
                        target.Append(' ', 3);
                        target.Append(chars);
                        chars.Clear();
                    }
                    target.AppendLine();
                    target.Append(' ', indent << 1);
                    target.AppendFormat("{0:x4}  ", i);
                }
                if ((i - 8) % 16 == 0)
                {
                    target.Append(" ");
                }
                if (i < length)
                {
                    byte b = value[offset + i];
                    target.AppendFormat("{0:x2} ", b);

                    char ch = (char)b;
                    bool replace = char.IsControl(ch) || ch > 0x7f;
                    chars.Append(replace ? '.' : ch);
                }
                else
                {
                    target.Append(' ', 3);
                    chars.Append(' ');
                }
            }
            if (chars.Length > 0)
            {
                target.Append(' ', 2);
                target.Append(chars);
            }
        }
    }
}
