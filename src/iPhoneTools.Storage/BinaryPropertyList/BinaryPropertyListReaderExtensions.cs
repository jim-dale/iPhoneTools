using System.IO;

namespace iPhoneTools
{
    public static class BinaryPropertyListReaderExtensions
    {
        public static object LoadFrom(this BinaryPropertyListReader item, string path)
        {
            object result = default;

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                result = item.LoadFrom(stream);
            }

            return result;
        }

        public static object LoadFrom(this BinaryPropertyListReader item, byte[] data)
        {
            object result = default;

            using (var stream = new MemoryStream(data, false))
            {
                result = item.LoadFrom(stream);
            }

            return result;
        }

        public static object LoadFrom(this BinaryPropertyListReader item, Stream stream)
        {
            PropertyContext result = default;

            using (var reader = new BinaryReader(stream))
            {
                result = item.LoadFrom(reader);
            }

            return result.Value;
        }
    }
}
