using System.Collections.Generic;

namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddInfoPropertyListFile(this AppContext result, string path, bool optional = false)
        {
            return AddFromFile(result, path, optional, (context) =>
            {
                var reader = new XmlPropertyListReader();

                var obj = reader.LoadFrom(path);
                if (obj is Dictionary<string, object> root)
                {
                    result.InfoPropertyList = root;
                }
            });
        }

        public static AppContext AddInfoProperties(this AppContext result)
        {
            result.InfoProperties = new BackupInfoProperties()
                .AddPropertyList(result.InfoPropertyList);

            return result;
        }
    }
}
