using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace iPhoneTools
{
    public class MbdbReader
    {
        private static readonly Dictionary<string, object> EmptyPropertyDictionary = new Dictionary<string, object>();

        public Mbdb LoadFrom(BinaryReader reader)
        {
            Mbdb result = default;

            if (reader.BaseStream.Length >= MbdbConstants.MinimumViableLength)
            {
                result = new Mbdb();

                result.MagicNumber = reader.ReadAsciiString(4);
                result.MajorVersion = reader.ReadByte();
                result.MinorVersion = reader.ReadByte();

                if (result.IsSupportedFormat())
                {
                    long length = reader.BaseStream.Length;
                    long position = reader.BaseStream.Position;

                    var entries = new List<MbdbEntry>();

                    while (position < length)
                    {
                        var entry = ReadMbdbEntry(reader);

                        entries.Add(entry);

                        position = reader.BaseStream.Position;
                    }

                    if (entries.Count > 0)
                    {
                        result.Items = entries.ToArray();
                    }
                }
            }

            return result;
        }

        private MbdbEntry ReadMbdbEntry(BinaryReader reader)
        {
            var result = new MbdbEntry
            {
                Domain = ReadString(reader),
                RelativePath = ReadString(reader),
                Target = ReadString(reader),
                FileContentsHash = ReadData(reader),
                WrappedKey = ReadEncryptionKey(reader),
                Mode = reader.ReadUInt16BigEndian(),
                InodeNumber = reader.ReadUInt64BigEndian(),
                UserId = reader.ReadUInt32BigEndian(),
                GroupId = reader.ReadUInt32BigEndian(),
                LastModifiedTime = ReadDateTimeOffset(reader),
                LastAccessedTime = ReadDateTimeOffset(reader),
                CreatedTime = ReadDateTimeOffset(reader),
                FileLength = reader.ReadInt64BigEndian(),
                Flags = reader.ReadByte(),
            };

            result.Properties = ReadProperties(reader);

            return result;
        }

        private IReadOnlyDictionary<string, object> ReadProperties(BinaryReader reader)
        {
            var result = EmptyPropertyDictionary;

            var propertyCount = reader.ReadByte();
            if (propertyCount > 0)
            {
                result = new Dictionary<string, object>();

                for (int i = 0; i < propertyCount; i++)
                {
                    var name = ReadString(reader);
                    var value = ReadAsciiStringOrData(reader);

                    result.Add(name, value);
                }
            }

            return result;
        }

        private DateTimeOffset ReadDateTimeOffset(BinaryReader reader)
        {
            long seconds = reader.ReadUInt32BigEndian();

            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        private string ReadString(BinaryReader reader)
        {
            var result = string.Empty;

            int size = reader.ReadUInt16BigEndian();
            if (size != 0xFFFF && size > 0)
            {
                result = reader.ReadUtf8String(size);
            }

            return result;
        }

        private WrappedKey ReadEncryptionKey(BinaryReader reader)
        {
            const int PrefixLength = 4;

            WrappedKey result = default;

            int size = reader.ReadUInt16BigEndian();
            if (size != 0xFFFF && size > PrefixLength)
            {
                int keySize = size - PrefixLength;

                result = new WrappedKey
                {
                    Unknown = reader.ReadBytes(PrefixLength),
                    Key = reader.ReadBytes(keySize),
                };
            }

            return result;
        }

        private object ReadAsciiStringOrData(BinaryReader reader)
        {
            object result = default;

            byte[] data = ReadData(reader);
            if (data != null)
            {
                if (IsAsciiString(data))
                {
                    result = Encoding.ASCII.GetString(data);
                }
                else
                {
                    result = data;
                }
            }

            return result;
        }

        private static bool IsAsciiString(byte[] data)
        {
            bool result = true;

            foreach (var bt in data)
            {
                char ch = (char)bt;
                if (Char.IsControl(ch))
                {
                    result = false;
                }
            }

            return result;
        }

        private byte[] ReadData(BinaryReader reader)
        {
            byte[] result = default;

            int size = reader.ReadUInt16BigEndian();
            if (size != 0xFFFF && size > 0)
            {
                result = reader.ReadBytes(size);
            }

            return result;
        }
    }
}
