namespace iPhoneTools
{
    public class BackupManifestEntry
    {
        public string Domain { get; set; }
        public string RelativePath { get; set; }
        public byte[] EncryptionKey { get; set; }
    }
}
