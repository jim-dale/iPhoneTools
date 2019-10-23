
namespace iPhoneTools
{
    public static partial class AppContextExtensions
    {
        public static AppContext AddInfoPropertyListFile(this AppContext result, string path)
        {
            result.InfoPropertyList = new XmlPropertyListReader()
                .LoadReadOnlyDictionaryFrom(path);

            return result;
        }

        public static AppContext AddInfoProperties(this AppContext result)
        {
            result.InfoProperties = new BackupInfoProperties()
                .AddPropertyList(result.InfoPropertyList);

            return result;
        }
    }
}
