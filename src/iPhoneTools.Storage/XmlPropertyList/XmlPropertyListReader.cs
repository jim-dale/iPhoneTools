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

        internal object ParsePropertyList(XDocument item)
        {
            var dictionary = item.Element(PListTag).Element(DictionaryTag);

            return ParseElement(dictionary);
        }

        private object ParseElement(XElement item)
        {
            var name = item.Name.LocalName.ToLowerInvariant();

            return ParseElement(item, name);
        }

        private object ParseElement(XElement item, string typeName)
        {
            object result = typeName switch
            {
                TrueTag => true,
                FalseTag => false,
                StringTag => item.Value,
                DateTag => ParseDate(item.Value),
                IntegerTag => ParseInteger(item.Value),
                RealTag => ParseDouble(item.Value),
                DictionaryTag => ParseDictionary(item),
                ArrayTag => ParseArray(item),
                DataTag => ParseData(item),
                _ => throw new InvalidDataException("Unknown property type " + typeName),
            };

            return result;
        }

        private Dictionary<string, object> ParseDictionary(XElement element)
        {
            var result = new Dictionary<string, object>();

            var key = string.Empty;
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
