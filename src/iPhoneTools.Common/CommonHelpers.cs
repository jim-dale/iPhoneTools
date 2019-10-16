using System;
using System.Security.Cryptography;
using System.Text;

namespace iPhoneTools
{
    public static partial class CommonHelpers
    {
        public static readonly DateTimeOffset MacEpoch = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);
        public static readonly long MachEpocSeconds = 63113904000;

        public static DateTimeOffset ConvertFromMacTime(double seconds)
        {
            DateTimeOffset result = default;

            if (DateTimeOffset.MinValue.Ticks < (MachEpocSeconds + seconds))
            {
                result = MacEpoch.AddSeconds(seconds);
            }

            return result;
        }

        public static DateTimeOffset ConvertFromMacTime(long value)
        {
            return (value < MachEpocSeconds) ? MacEpoch.AddSeconds(value) : MacEpoch.AddMilliseconds(value / 1_000_000);
        }

        public static ManifestEntryType GetManifestEntryTypeFromMode(int mode)
        {
            ManifestEntryType result;

            mode = mode & 0xE000;
            switch (mode)
            {
                case 0xA000:
                    result = ManifestEntryType.Symlink;
                    break;
                case 0x8000:
                    result = ManifestEntryType.File;
                    break;
                case 0x4000:
                    result = ManifestEntryType.Directory;
                    break;
                default:
                    result = ManifestEntryType.NotRecognised;
                    break;
            }

            return result;
        }

        public static string GetManifestEntryModeAsString(int mode)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(((mode & 0x0004) == 0x0004) ? "r" : "-");
            builder.Append(((mode & 0x0002) == 0x0002) ? "w" : "-");
            builder.Append(((mode & 0x0001) == 0x0001) ? "x" : "-");

            return builder.ToString();
        }

        public static string GetManifestEntryMode(int mode)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(GetManifestEntryModeAsString(mode >> 6));
            builder.Append(' ');
            builder.Append(GetManifestEntryModeAsString(mode >> 3));
            builder.Append(' ');
            builder.Append(GetManifestEntryModeAsString(mode));

            return builder.ToString();
        }

        public static string Sha1HashAsHexString(string input)
        {
            string result = default;

            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));

                result = ByteArrayToHexString(hash);
            }

            return result;
        }

        public static string ByteArrayToHexString(byte[] value)
        {
            var sb = new StringBuilder(value.Length * 2);

            foreach (byte b in value)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
