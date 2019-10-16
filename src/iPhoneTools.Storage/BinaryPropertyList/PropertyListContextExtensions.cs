using System;
using System.Buffers.Binary;
using System.IO;

namespace iPhoneTools
{
    public static class PropertyListContextExtensions
    {
        private const string MagicNumber = "bplist";
        private const string VersionNumber = "00";

        public static bool IsSupportedBinaryPropertyList(this PropertyListContext item)
        {
            return string.Equals(MagicNumber, item.MagicNumber, StringComparison.OrdinalIgnoreCase)
                && string.Equals(VersionNumber, item.FileFormatVersion, StringComparison.OrdinalIgnoreCase);
        }

        public static PropertyListContext HeaderFromBinaryReader(this PropertyListContext result, BinaryReader reader)
        {
            result.MagicNumber = reader.ReadAsciiString(6);
            result.FileFormatVersion = reader.ReadAsciiString(2);

            return result;
        }

        public static PropertyListContext TrailerFromBinaryReader(this PropertyListContext result, BinaryReader reader)
        {
            reader.BaseStream.Seek(-32, SeekOrigin.End);

            var data = reader.ReadBytes(32);

            result.SortVersion = data[5];
            result.OffsetTableOffsetSize = data[6];
            result.ObjectRefSize = data[7];
            result.NumObjects = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(8, 8));
            result.TopObjectOffset = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(16, 8));
            result.OffsetTableStart = BinaryPrimitives.ReadInt64BigEndian(data.AsSpan(24, 8));

            return result;
        }

        public static PropertyListContext OffsetTableFromBinaryReader(this PropertyListContext result, BinaryReader reader)
        {
            reader.BaseStream.Seek(result.OffsetTableStart, SeekOrigin.Begin);

            result.Offsets = reader.ReadIntArrayBigEndian(result.OffsetTableOffsetSize, (int)result.NumObjects);

            return result;
        }
    }
}
