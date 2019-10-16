using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace iPhoneTools
{
    public class XmlPropertyListReader
    {
        private const string PListTag = "plist";
        private const string TrueTag = "true";
        private const string FalseTag = "false";
        private const string StringTag = "string";
        private const string DateTag = "date";
        private const string IntegerTag = "integer";
        private const string RealTag = "real";
        private const string DictionaryTag = "dict";
        private const string DictionaryKeyTag = "key";
        private const string ArrayTag = "array";
        private const string DataTag = "data";

        private XDocument _doc;

        public object LoadFrom(string path)
        {
            _doc = XDocument.Load(path);

            var dict = _doc.Element(PListTag).Element(DictionaryTag);

            return ParseElement(dict);
        }

        private object ParseElement(XElement item)
        {
            var name = item.Name.LocalName.ToLowerInvariant();

            return ParseElement(item, name);
        }

        private object ParseElement(XElement item, string typeName)
        {
            object result;

            switch (typeName)
            {
                case TrueTag:
                    result = true;
                    break;
                case FalseTag:
                    result = false;
                    break;
                case StringTag:
                    result = item.Value;
                    break;
                case DateTag:
                    result = ParseDate(item.Value);
                    break;
                case IntegerTag:
                    result = ParseInteger(item.Value);
                    break;
                case RealTag:
                    result = ParseDouble(item.Value);
                    break;
                case DictionaryTag:
                    result = ParseDictionary(item);
                    break;
                case ArrayTag:
                    result = ParseArray(item);
                    break;
                case DataTag:
                    result = ParseData(item);
                    break;
                default:
                    throw new InvalidDataException("Unknown property type " + typeName);
            }

            return result;
        }

        private Dictionary<string, object> ParseDictionary(XElement element)
        {
            var result = new Dictionary<string, object>();

            string key = string.Empty;
            foreach (var item in element.Elements())
            {
                var name = item.Name.LocalName.ToLowerInvariant();

                if (string.Equals(DictionaryKeyTag, name, StringComparison.OrdinalIgnoreCase))
                {
                    key = item.Value;
                }
                else
                {
                    var value = ParseElement(item, name);

                    result.Add(key, value);

                    key = string.Empty;
                }
            }

            return result;
        }

        private DateTimeOffset ParseDate(string value)
        {
            return DateTimeOffset.Parse(value);
        }

        private int ParseInteger(string value)
        {
            return Convert.ToInt32(value, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        private double ParseDouble(string value)
        {
            return Convert.ToDouble(value, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        private object[] ParseArray(XElement element)
        {
            var items = new List<object>();

            foreach (var item in element.Elements())
            {
                var value = ParseElement(item);

                items.Add(value);
            }

            return items.ToArray();
        }

        private byte[] ParseData(XElement element)
        {
            return Convert.FromBase64String(element.Value);
        }
    }
}
