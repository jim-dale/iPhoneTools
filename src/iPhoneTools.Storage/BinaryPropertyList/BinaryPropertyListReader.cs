using System.Collections.Generic;
using System.IO;

/// <remarks>
/// HEADER
/// magic number ("bplist")
/// file format version
/// 
/// OBJECT TABLE
///     variable-sized objects
/// 
///     Object Formats (marker byte followed by additional info in some cases)
///     null    0000 0000
///     bool    0000 1000                           // false
///     bool    0000 1001                           // true
///     fill    0000 1111                           // fill byte
///     int     0001 nnnn ...                       // # of bytes is 2^nnnn, big-endian bytes
///     real    0010 nnnn ...                       // # of bytes is 2^nnnn, big-endian bytes
///     date    0011 0011 ...                       // 8 byte float follows, big-endian bytes
///     data    0100 nnnn   [int] ...               // nnnn is number of bytes unless 1111 then int count follows, followed by bytes
///     string  0101 nnnn   [int] ...               // ASCII string, nnnn is # of chars, else 1111 then int count, then bytes
///     string  0110 nnnn   [int] ...               // Unicode string, nnnn is # of chars, else 1111 then int count, then big-endian 2-byte uint16_t
///             0111 xxxx                           // unused
///     uid     1000 nnnn ...                       // nnnn+1 is # of bytes
///             1001 xxxx                           // unused
///     array   1010 nnnn   [int] objref*           // nnnn is count, unless '1111', then int count follows
///             1011 xxxx                           // unused
///     set     1100 nnnn   [int] objref*           // nnnn is count, unless '1111', then int count follows
///     dict    1101 nnnn   [int] keyref* objref*   // nnnn is count, unless '1111', then int count follows
///             1110 xxxx                           // unused
///             1111 xxxx                           // unused
/// 
/// OFFSET TABLE
/// 	list of ints, byte size of which is given in trailer
/// 	-- these are the byte offsets into the file
/// 	-- number of these is in the trailer
/// 
/// TRAILER
/// 	byte size of offset ints in offset table
/// 	byte size of object refs in arrays and dicts
/// 	number of offsets in offset table (also is number of objects)
/// 	element # in offset table which is top level object
/// 	offset table offset
/// </remarks>

namespace iPhoneTools
{
    public class BinaryPropertyListReader
    {
        private const int MinimumViableFileSize = 40;

        internal object LoadFrom(BinaryReader reader)
        {
            return Parse(reader).Value;
        }


        private PropertyContext Parse(BinaryReader reader)
        {
            var result = PropertyContext.Empty;

            if (reader.BaseStream.Length >= MinimumViableFileSize)
            {
                var context = new PropertyListContext()
                    .HeaderFromBinaryReader(reader);

                if (context.IsSupportedBinaryPropertyList())
                {
                    context = context.TrailerFromBinaryReader(reader)
                        .OffsetTableFromBinaryReader(reader);

                    result = ParseObjectByOffsetIndex(reader, context, (int)context.TopObjectOffset);
                }
            }

            return result;
        }

        private PropertyContext ParseObjectByOffsetIndex(BinaryReader reader, PropertyListContext ctx, int index)
        {
            return ParseObjectByPosition(reader, ctx, ctx.Offsets[index]);
        }

        private PropertyContext ParseObjectByPosition(BinaryReader reader, PropertyListContext ctx, long position)
        {
            reader.BaseStream.Seek(position, SeekOrigin.Begin);

            return ParseObject(reader, ctx, position);
        }

        private PropertyContext ParseObject(BinaryReader reader, PropertyListContext ctx, long position)
        {
            PropertyContext result;

            var marker = reader.ReadByte();
            var msn = marker & 0xf0;
            var lsn = marker & 0x0f;

            switch (msn)
            {
                case 0b0000_0000:
                    switch (lsn)
                    {
                        case 0b0000_0000:
                            result = new PropertyContext(position, 0, 1, PropertyType.Null, null);
                            break;
                        case 0b0000_1000:
                            result = new PropertyContext(position, 0, 1, PropertyType.Bool, false);
                            break;
                        case 0b0000_1001:
                            result = new PropertyContext(position, 0, 1, PropertyType.Bool, true);
                            break;
                        case 0b0000_1111:
                            result = new PropertyContext(position, 0, 1, PropertyType.Fill, null);
                            break;
                        default:
                            throw new InvalidDataException("Unrecognised object type " + marker.ToString("x2"));
                    }
                    break;
                case 0b0001_0000:
                    result = ReadInteger(reader, position, lsn);
                    break;
                case 0b0010_0000:
                    result = ReadReal(reader, position, lsn);
                    break;
                case 0b0011_0000:
                    result = ReadDate(reader, position, lsn);
                    break;
                case 0b0100_0000:
                    result = ReadData(reader, position, lsn);
                    break;
                case 0b0101_0000:
                    result = ReadAsciiString(reader, position, lsn);
                    break;
                case 0b0110_0000:
                    result = ReadUnicodeString(reader, position, lsn);
                    break;
                case 0b1000_0000:
                    result = ReadUid(reader, position, lsn);
                    break;
                case 0b1010_0000:
                    result = ReadArray(reader, ctx, position, lsn);
                    break;
                case 0b1100_0000:
                    result = ReadSet(reader, ctx, position, lsn);
                    break;
                case 0b1101_0000:
                    result = ReadDictionary(reader, ctx, position, lsn);
                    break;
                default:
                    throw new InvalidDataException("Unrecognised object type " + marker.ToString("x2"));
            }

            return result;
        }

        /// <remarks>
        /// int 0001 nnnn...
        /// # of bytes is 2^nnnn, big-endian bytes
        /// </remarks>
        private PropertyContext ReadInteger(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            int size = 1 << count;
            object value = size switch
            {
                1 => reader.ReadByte(),
                2 => reader.ReadUInt16BigEndian(),
                4 => reader.ReadUInt32BigEndian(),
                8 => reader.ReadInt64BigEndian(),
                16 => reader.ReadBytes(size),
                _ => throw new InvalidDataException("Unsupported integer value size"),
            };
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.Integer, value);
        }

        /// <remarks>
        /// real    0010 nnnn...
        /// # of bytes is 2^nnnn, big-endian bytes
        /// </remarks>
        private PropertyContext ReadReal(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            int size = 1 << count;
            object value = size switch
            {
                4 => reader.ReadSingleBigEndian(),
                8 => reader.ReadDoubleBigEndian(),
                _ => throw new InvalidDataException("Unsupported real value size"),
            };
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.Real, value);
        }

        /// <remarks>
        /// date    0011 0011...
        /// 8 byte float follows, big-endian bytes
        /// </remarks>
        private PropertyContext ReadDate(BinaryReader reader, long position, int count)
        {
            var seconds = reader.ReadDoubleBigEndian();
            var value = CommonHelpers.ConvertFromMacTime(seconds);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.Date, value);
        }

        /// <remarks>
        /// data    0100 nnnn[int]...
        /// nnnn is number of bytes unless 1111 then int count follows, followed by bytes
        /// </remarks>
        private PropertyContext ReadData(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            var value = reader.ReadBytes(count);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.Data, value);
        }

        /// <remarks>
        /// string  0101 nnnn[int]...
        /// ASCII string, nnnn is # of chars, else 1111 then int count, then bytes
        /// </remarks>
        private PropertyContext ReadAsciiString(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            var value = reader.ReadAsciiString(count);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.AsciiString, value);
        }

        /// <remarks>
        /// string  0110 nnnn[int]...
        /// Unicode string, nnnn is # of chars, else 1111 then int count, then big-endian 2-byte uint16_t
        /// </remarks>
        private PropertyContext ReadUnicodeString(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            int size = count << 1;
            var value = reader.ReadUnicodeStringBigEndian(size);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.UnicodeString, value);
        }

        /// <remarks>
        /// uid    1000 nnnn...
        /// nnnn+1 is # of bytes
        /// </remarks>
        private PropertyContext ReadUid(BinaryReader reader, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            int size = count + 1;
            var value = reader.ReadLongBigEndian(size);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            return new PropertyContext(position, count, totalSize, PropertyType.Uid, value);
        }

        /// <remarks>
        /// array   1010 nnnn[int], objref *
        /// nnnn is count, unless '1111', then int count follows
        /// </remarks>
        private PropertyContext ReadArray(BinaryReader reader, PropertyListContext ctx, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            var valueRefs = reader.ReadIntArrayBigEndian(ctx.ObjectRefSize, count);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            var value = ParseArrayCore(reader, ctx, count, valueRefs);

            return new PropertyContext(position, count, totalSize, PropertyType.Array, value);
        }

        /// <remarks>
        /// set 1100 nnnn[int], objref*
        /// nnnn is count, unless '1111', then int count follows
        /// </remarks>
        private PropertyContext ReadSet(BinaryReader reader, PropertyListContext ctx, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            var valueRefs = reader.ReadIntArrayBigEndian(ctx.ObjectRefSize, count);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            var value = ParseArrayCore(reader, ctx, count, valueRefs);

            return new PropertyContext(position, count, totalSize, PropertyType.Set, value);
        }

        /// <remarks>
        /// dict    1101 nnnn[int],keyref*,objref*
        /// nnnn is count, unless '1111', then int count follows
        /// </remarks>
        private PropertyContext ReadDictionary(BinaryReader reader, PropertyListContext ctx, long position, int count)
        {
            count = ReadObjectSize(reader, count);

            var keyRefs = reader.ReadIntArrayBigEndian(ctx.ObjectRefSize, count);
            var valueRefs = reader.ReadIntArrayBigEndian(ctx.ObjectRefSize, count);
            int totalSize = reader.GetOffsetFromCurrentPosition(position);

            var value = ParseDictionaryCore(reader, ctx, count, keyRefs, valueRefs);

            return new PropertyContext(position, count, totalSize, PropertyType.Dictionary, value);
        }

        private int ReadObjectSize(BinaryReader reader, int size)
        {
            int result = size;

            if (result == 0x0f)
            {
                var data = reader.ReadByte();
                var msn = data & 0xf0;
                var lsn = data & 0x0f;

                if (msn == 0x10)
                {
                    size = 1 << lsn;

                    result = reader.ReadIntBigEndian(size);
                }
                else
                {
                    throw new InvalidDataException("Unexpected value size");
                }
            }

            return result;
        }

        private object ParseArrayCore(BinaryReader reader, PropertyListContext ctx, int count, int[] valueRefs)
        {
            var result = new object[count];
            for (int i = 0; i < count; i++)
            {
                var valueRef = valueRefs[i];
                var valueContext = ParseObjectByOffsetIndex(reader, ctx, valueRef);

                result[i] = valueContext.Value;
            }

            return result;
        }

        private object ParseDictionaryCore(BinaryReader reader, PropertyListContext ctx, int count, int[] keyRefs, int[] valueRefs)
        {
            var result = new Dictionary<string, object>();
            for (int i = 0; i < count; i++)
            {
                var keyRef = keyRefs[i];
                var valueRef = valueRefs[i];

                var keyContext = ParseObjectByOffsetIndex(reader, ctx, keyRef);
                var valueContext = ParseObjectByOffsetIndex(reader, ctx, valueRef);

                var key = (string)keyContext.Value;
                var value = valueContext.Value;

                result.Add(key, value);
            }

            return result;
        }
    }
}
