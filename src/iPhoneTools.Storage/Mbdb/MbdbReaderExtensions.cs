using System.IO;

namespace iPhoneTools
{
    public static class MbdbReaderExtensions
    {
        public static Mbdb LoadFrom(this MbdbReader item, string path)
        {
            Mbdb result = default;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = item.LoadFrom(stream);
            }

            return result;
        }

        public static Mbdb LoadFrom(this MbdbReader item, byte[] data)
        {
            Mbdb result = default;

            using (var stream = new MemoryStream(data, false))
            {
                result = item.LoadFrom(stream);
            }

            return result;
        }

        public static Mbdb LoadFrom(this MbdbReader item, Stream stream)
        {
            Mbdb result = default;

            using (var reader = new BinaryReader(stream))
            {
                result = item.LoadFrom(reader);
            }

            return result;
        }
    }
}
