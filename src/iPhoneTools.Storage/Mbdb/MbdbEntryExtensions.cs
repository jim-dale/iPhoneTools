
namespace iPhoneTools
{
    public static partial class MbdbEntryExtensions
    {
        public static string GetSha1HashAsHexString(this MbdbEntry item)
        {
            string result = default;

            if (string.IsNullOrWhiteSpace(item.Domain) == false && string.IsNullOrWhiteSpace(item.RelativePath) == false)
            {
                var file = item.Domain + "-" + item.RelativePath;

                 result = CommonHelpers.Sha1HashAsHexString(file);
            }

            return result;
        }
    }
}
