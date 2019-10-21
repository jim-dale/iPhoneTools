
namespace iPhoneTools
{
    public static class MbdbEntryConverter
    {
        public static ManifestEntry ConvertToManifestEntry(this MbdbEntry item, ManifestEntryType includeType, bool isEncrypted)
        {
            ManifestEntry result = default;

            var entryType = CommonHelpers.GetManifestEntryTypeFromMode(item.Mode);
            if (entryType == includeType)
            {
                var id = item.GetSha1HashAsHexString();

                result = new ManifestEntry
                {
                    Id = id,
                    Domain = item.Domain,
                    RelativePath = item.RelativePath,
                    EntryType = entryType,
                };

                if (isEncrypted)
                {
                    result.ProtectionClass = (ProtectionClass)item.Flags;
                    result.WrappedKey = item.WrappedKey;
                }
            }
            return result;
        }
    }
}
