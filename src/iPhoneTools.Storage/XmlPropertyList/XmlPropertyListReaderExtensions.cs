using System.Collections.Generic;
using System.IO;

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
    }
}
