
namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddManifestPropertyListFile(this AppContext result, string path)
        {
            result.ManifestPropertyList = new BinaryPropertyListReader()
                .LoadReadOnlyDictionaryFrom(path);

            return result;
        }

        public static AppContext AddManifestProperties(this AppContext result)
        {
            result.ManifestProperties = new BackupManifestProperties()
                .AddPropertyList(result.ManifestPropertyList);

            return result;
        }
    }
}
