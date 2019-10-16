﻿using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace iPhoneTools
{
    public static partial class BinaryReaderExtensions
    {
        // 1201 is the encoding for utf-16BE, Unicode (Big-Endian)
        private static readonly Encoding _utf16beEncoding = Encoding.GetEncoding(1201);

        public static int GetOffsetFromCurrentPosition(this BinaryReader item, long position)
        {
            long currentPosition = item.BaseStream.Position;

            return (int)(currentPosition - position);
        }

        public static string ReadAsciiString(this BinaryReader item, int size)
        {
            var data = item.ReadBytes(size);

            return Encoding.ASCII.GetString(data);
        }

        public static string ReadUtf8String(this BinaryReader item, int size)
        {
            var data = item.ReadBytes(size);

            return Encoding.UTF8.GetString(data);
        }

        public static string ReadUnicodeStringBigEndian(this BinaryReader item, int size)
        {
            var data = item.ReadBytes(size);

            return _utf16beEncoding.GetString(data);
        }

        public static int[] ReadIntArrayBigEndian(this BinaryReader item, int size, int count)
        {
            var result = new int[count];
            for (int i = 0; i < count; i++)
            {
                result[i] = ReadIntBigEndian(item, size);
            }

            return result;
        }

        public static int ReadIntBigEndian(this BinaryReader item, int size)
        {
            int result;

            switch (size)
            {
                case 1:
                    result = item.ReadByte();
                    break;
                case 2:
                    result = item.ReadUInt16BigEndian();
                    break;
                case 4:
                    result = item.ReadInt32BigEndian();
                    break;
                default:
                    throw new InvalidDataException("Unsupported integer value size");
            }

            return result;
        }

        public static long ReadLongBigEndian(this BinaryReader item, int size)
        {
            long result;

            switch (size)
            {
                case 1:
                    result = item.ReadByte();
                    break;
                case 2:
                    result = item.ReadUInt16BigEndian();
                    break;
                case 4:
                    result = item.ReadUInt32BigEndian();
                    break;
                case 8:
                    result = item.ReadInt64BigEndian();
                    break;
                default:
                    throw new InvalidDataException("Unsupported integer value size");
            }

            return result;
        }

        public static object ReadRealBigEndian(this BinaryReader item, int size)
        {
            object result;

            switch (size)
            {
                case 4:
                    result = item.ReadSingleBigEndian();
                    break;
                case 8:
                    result = item.ReadDoubleBigEndian();
                    break;
                default:
                    throw new InvalidDataException("Unsupported real value size");
            }

            return result;
        }

        public static short ReadInt16BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(2);
            return BinaryPrimitives.ReadInt16BigEndian(data.AsSpan());
        }

        public static ushort ReadUInt16BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(2);
            return BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan());
        }

        public static int ReadInt32BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(4);
            return BinaryPrimitives.ReadInt32BigEndian(data.AsSpan());
        }

        public static uint ReadUInt32BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(4);
            return BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan());
        }

        public static long ReadInt64BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(8);
            return BinaryPrimitives.ReadInt64BigEndian(data.AsSpan());
        }

        public static ulong ReadUInt64BigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(8);
            return BinaryPrimitives.ReadUInt64BigEndian(data.AsSpan());
        }

        public static float ReadSingleBigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(4);
            Array.Reverse(data);
            return BitConverter.ToSingle(data, 0);
        }

        public static double ReadDoubleBigEndian(this BinaryReader item)
        {
            var data = item.ReadBytes(8);
            Array.Reverse(data);
            return BitConverter.ToDouble(data, 0);
        }
    }
}
