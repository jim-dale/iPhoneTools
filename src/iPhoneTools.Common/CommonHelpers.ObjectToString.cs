using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace iPhoneTools
{
    public static partial class CommonHelpers
    {
        public static string ObjectToString(object item)
        {
            return ObjectToString(item, 0);
        }

        public static string ObjectToString(object item, int indent)
        {
            var builder = new StringBuilder();

            ObjectToString(item, indent, builder);

            return builder.ToString();
        }

        public static void ObjectToString(object value, int indent, StringBuilder target)
        {
            if (IsSimpleType(value))
            {
                target.Append(SimpleValueToString(value));
            }
            else
            {
                switch (value)
                {
                    case byte[] data:
                        ByteArrayToString(data, indent, target);
                        break;
                    case IDictionary<string, object> dict:
                        DictionaryToString(dict, indent, target);
                        break;
                    case object[] arr:
                        ArrayToString(arr, indent, target);
                        break;
                    default:
                        throw new InvalidDataException("Unsupported property type " + value.GetType().Name);
                }
            }
        }

        private static void ByteArrayToString(byte[] item, int indent, StringBuilder target)
        {
            target.Append("Binary Data");
            ByteArrayToDebugString(item, indent, target);
        }

        private static void DictionaryToString(IDictionary<string, object> items, int indent, StringBuilder target)
        {
            target.Append("Dictionary");
            ++indent;

            foreach (var item in items)
            {
                var valueStr = ObjectToString(item.Value, indent);

                target.AppendLine();
                target.Append(' ', indent << 1);
                target.Append($"[{item.Key}]={valueStr}");
            }
        }

        private static void ArrayToString(object[] items, int indent, StringBuilder target)
        {
            target.Append("Array");
            ++indent;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var valueStr = ObjectToString(item, indent);

                target.AppendLine();
                target.Append(' ', indent << 1);
                target.Append($"[{i}]={valueStr}");
            }
        }

        private static bool IsSimpleType(object value)
        {
            return (value is null || value is string || value is bool
                || value is byte || value is short || value is int || value is long
                || value is float || value is double
                || value is DateTimeOffset
                );
        }

        private static string SimpleValueToString(object value)
        {
            string result;

            if (value is null)
            {
                result = "(null)";
            }
            else if (value is string str)
            {
                result = "\"" + str + "\" (" + value.GetType().Name + ")";
            }
            else
            {
                result = $"{value} ({value.GetType().Name})";
            }

            return result;
        }
    }
}
