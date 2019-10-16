using System.Collections.Generic;

namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddStatusPropertyListFile(this AppContext result, string path, bool optional = false)
        {
            return AddFromFile(result, path, optional, (context) =>
            {
                var reader = new BinaryPropertyListReader();

                var obj = reader.LoadFrom(path);
                if (obj is Dictionary<string, object> root)
                {
                    result.StatusPropertyList = root;
                }
            });
        }

        public static AppContext AddStatusProperties(this AppContext result)
        {
            result.StatusProperties = new BackupStatusProperties()
                .AddPropertyList(result.StatusPropertyList);

            return result;
        }
    }
}
