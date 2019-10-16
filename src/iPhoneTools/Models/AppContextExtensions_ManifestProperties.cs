using System.Collections.Generic;

namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddManifestPropertyListFile(this AppContext result, string path, bool optional = false)
        {
            return AddFromFile(result, path, optional, (context) =>
            {
                var reader = new BinaryPropertyListReader();

                var obj = reader.LoadFrom(path);
                if (obj is Dictionary<string, object> root)
                {
                    result.ManifestPropertyList = root;
                }
            });
        }

        public static AppContext AddManifestProperties(this AppContext result)
        {
            result.ManifestProperties = new BackupManifestProperties()
                .AddPropertyList(result.ManifestPropertyList);

            return result;
        }
    }
}
