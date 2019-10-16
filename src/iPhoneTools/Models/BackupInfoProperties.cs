using System;

namespace iPhoneTools
{
    public class BackupInfoProperties
    {
        public string BuildVersion { get; set; }
        public string DeviceName { get; set; }
        public string DisplayName { get; set; }
        public Guid Guid { get; set; }
        public string IccId { get; set; }
        public string Imei { get; set; }
        public DateTimeOffset LastBackupDate { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public string ProductVersion { get; set; }
        public string SerialNumber { get; set; }
        public string TargetIdentifier { get; set; }
        public string TargetType { get; set; }
        public string UniqueIdentifier { get; set; }
        public string ITunesVersion { get; internal set; }
    }
}
