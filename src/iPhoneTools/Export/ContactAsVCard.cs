using System;
using System.Text;
using System.IO;

namespace iPhoneTools
{
    public static class ContactAsVCard
    {
        public static void Save(ContactDbEntry item)
        {
            string fomattedName = GetFormattedName(item);

            var outputFile = fomattedName + ".vcard";

            using (var stream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                SerializeContactToStream(item, stream, fomattedName);
            }
        }

        private static void SerializeContactToStream(ContactDbEntry item, Stream stream, string formattedName)
        {
            SerializePropertyToStream(stream, "BEGIN", "VCARD");
            SerializePropertyToStream(stream, "VERSION", "2.1");
            SerializePropertyToStream(stream, "N", GetStructuredName(item));
            SerializePropertyToStream(stream, "FN", formattedName);

            if (string.IsNullOrEmpty(item.Organization) == false)
            {
                SerializePropertyToStream(stream, "ORG", item.Organization);
            }
            if (string.IsNullOrEmpty(item.JobTitle) == false)
            {
                SerializePropertyToStream(stream, "ORG", item.JobTitle);
            }
            if (string.IsNullOrEmpty(item.WorkPhone) == false)
            {
                SerializePropertyToStream(stream, "TEL;WORK;VOICE", item.WorkPhone);
            }
            if (string.IsNullOrEmpty(item.MobilePhone) == false)
            {
                SerializePropertyToStream(stream, "TEL;CELL;VOICE", item.MobilePhone);
            }
            if (string.IsNullOrEmpty(item.EMail) == false)
            {
                SerializePropertyToStream(stream, "EMAIL;PREF;INTERNET", item.EMail);
            }
            if (item.ModificationDate != DateTimeOffset.MinValue)
            {
                var dateStr = item.ModificationDate.ToUniversalTime().ToString("yyyyMMddTHHmmssZ");
                SerializePropertyToStream(stream, "REF", dateStr);
            }
            SerializePropertyToStream(stream, "END", "VCARD");
        }

        private static string GetFormattedName(ContactDbEntry item)
        {
            var result = StringExtensions.JoinIgnoreEmptyValues(" ", item.Prefix, item.First, item.Middle, item.Last, item.Suffix);
            if (string.IsNullOrWhiteSpace(result))
            {
                result = item.Organization;
            }

            return result;
        }

        private static string GetStructuredName(ContactDbEntry item)
        {
            return string.Join(";", item.Last, item.First, item.Middle, item.Prefix, item.Suffix);
        }

        private static void SerializePropertyToStream(Stream stream, string name, string value)
        {
            var data = GetPropertyAsBytes(name, value);
            WriteProperty(stream, data.Item1, data.Item2);
        }

        private static void WriteProperty(Stream stream, byte[] name, byte[] value)
        {
            stream.Write(name, 0, name.Length);
            stream.Write(value, 0, value.Length);
            stream.WriteByte(0x0d);
            stream.WriteByte(0x0a);
        }

        private static (byte[], byte[]) GetPropertyAsBytes(string name, string value)
        {
            var isValueASCII = IsASCII(value);

            if (isValueASCII == false)
            {
                name += ";CHARSET=utf-8";
            }
            name += ':';

            var nameData = StringAsAsciiBytes(name);
            var valueData = (isValueASCII) ? StringAsAsciiBytes(value) : StringAsUtf8Bytes(value);

            return (nameData, valueData);
        }

        private static bool IsASCII(string value)
        {
            var result = true;

            foreach (var ch in value)
            {
                if (ch >= 128)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private static byte[] StringAsAsciiBytes(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        private static byte[] StringAsUtf8Bytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
