
namespace iPhoneTools
{
    public class ManifestEntry
    {
        public string Id { get; set; }
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public ManifestEntryType EntryType { get; set; }
        public ProtectionClass ProtectionClass { get; set; }
        public WrappedKey WrappedKey { get; set; }
    }
}
