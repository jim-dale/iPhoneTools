using System;

namespace iPhoneTools
{
    public class BackupStatusProperties
    {
        public bool IsFullBackup { get; set; }
        public string Version { get; set; }
        public Guid Uuid { get; set; }
        public DateTimeOffset Date { get; set; }
        public string BackupState { get; set; }
        public string SnapshotState { get; set; }
    }
}
