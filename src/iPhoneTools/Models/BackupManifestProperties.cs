using System;

namespace iPhoneTools
{
    public class BackupManifestProperties
    {
        public string Version { get; set; }
        public DateTimeOffset Date { get; set; }
        public string SystemDomainsVersion { get; set; }
        public bool WasPasscodeSet { get; set; }
        public bool IsEncrypted { get; set; }
    }
}
