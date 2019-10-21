using System.Collections.Generic;

namespace iPhoneTools
{
    public static class ManifestDbEntryConverter
    {
        public static ManifestEntry ConvertToManifestEntry(this ManifestDbEntry item, ManifestEntryType includeType, bool isEncrypted)
        {
            ManifestEntry result = default;

            var propertyList = new BinaryPropertyListReader()
                .LoadFrom(item.Properties);

            if (propertyList != null && propertyList is IReadOnlyDictionary<string, object>)
            {
                dynamic properties = propertyList;

                var mode = (int)properties["$objects"][1]["Mode"];
                var entryType = CommonHelpers.GetManifestEntryTypeFromMode(mode);
                if (entryType == includeType)
                {
                    var id = item.FileID;

                    result = new ManifestEntry
                    {
                        Id = id,
                        Domain = item.Domain,
                        RelativePath = item.RelativePath,
                        EntryType = entryType,
                    };

                    if (isEncrypted)
                    {
                        var protectionClass = (ProtectionClass)properties["$objects"][1]["ProtectionClass"];
                        var index = (int)properties["$objects"][1]["EncryptionKey"];
                        var data = (byte[])properties["$objects"][index]["NS.data"];

                        result.ProtectionClass = protectionClass;
                        result.WrappedKey = WrappedKeyReader.Read(data);
                    }

                }
            }

            return result;
        }
    }
}
