using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace iPhoneTools
{
    public static class XmlPropertyListReaderExtensions
    {
        public static IReadOnlyDictionary<string, object> LoadReadOnlyDictionaryFrom(this XmlPropertyListReader item, string path)
        {
            var obj = item.LoadFrom(path);

            var result = obj as Dictionary<string, object>;
            if (obj is null)
            {
                throw new InvalidDataException();
            }

            return result;
        }

        public static object LoadFrom(this XmlPropertyListReader item, string path)
        {
            var doc = XDocument.Load(path);

            return item.ParsePropertyList(doc);
        }

        public static object Parse(this XmlPropertyListReader item, string text)
        {
            var doc = XDocument.Parse(text);

            return item.ParsePropertyList(doc);
        }
    }
}
