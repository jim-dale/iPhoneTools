using System;
using System.Collections.Generic;

namespace iPhoneTools
{
    public static class BackupStatusPropertiesExtensions
    {
        public static BackupStatusProperties AddPropertyList(this BackupStatusProperties result, IReadOnlyDictionary<string, object> items)
        {
            result.IsFullBackup = (bool)items["IsFullBackup"];
            result.Version = (string)items["Version"];
            result.Uuid = Guid.Parse((string)items["UUID"]);
            result.Date = (DateTimeOffset)items["Date"];
            result.BackupState = (string)items["BackupState"];
            result.SnapshotState = (string)items["SnapshotState"];

            return result;
        }
    }
}
