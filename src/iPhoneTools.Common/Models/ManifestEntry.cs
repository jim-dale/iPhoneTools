using System.Collections.Generic;

namespace iPhoneTools
{
    public class ManifestEntry
    {
        public string FileId { get; set; }
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public ManifestEntryType FileType { get; set; }
        public ProtectionClass ProtectionClass { get; set; }
        public WrappedKey WrappedKey { get; set; }

        public byte[] UnwrapEncryptionKey(IReadOnlyDictionary<ProtectionClass, byte[]> classKeys)
        {
            return WrappedKey.UnwrapKey(ProtectionClass, classKeys);
        }
    }
}
