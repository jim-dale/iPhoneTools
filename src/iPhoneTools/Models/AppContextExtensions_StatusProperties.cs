
namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddStatusPropertyListFile(this AppContext result, string path)
        {
            result.StatusPropertyList = new BinaryPropertyListReader()
                .LoadReadOnlyDictionaryFrom(path);

            return result;
        }

        public static AppContext AddStatusProperties(this AppContext result)
        {
            result.StatusProperties = new BackupStatusProperties()
                .AddPropertyList(result.StatusPropertyList);

            return result;
        }
    }
}
