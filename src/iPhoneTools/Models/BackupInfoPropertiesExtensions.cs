using System;
using System.Collections.Generic;

namespace iPhoneTools
{
    public static class BackupInfoPropertiesExtensions
    {
        public static BackupInfoProperties AddPropertyList(this BackupInfoProperties result, IReadOnlyDictionary<string, object> items)
        {
            result.BuildVersion = (string)items["Build Version"];
            result.DeviceName = (string)items["Device Name"];
            result.DisplayName = (string)items["Display Name"];
            result.Guid = Guid.Parse((string)items["GUID"]);
            result.IccId = (string)items["ICCID"];
            result.Imei = (string)items["IMEI"];
            result.LastBackupDate = (DateTimeOffset)items["Last Backup Date"];
            result.PhoneNumber = (string)items["Phone Number"];
            result.ProductName = (string)items["Product Name"];
            result.ProductType = (string)items["Product Type"];
            result.ProductVersion = (string)items["Product Version"];
            result.SerialNumber = (string)items["Serial Number"];
            result.TargetIdentifier = (string)items["Target Identifier"];
            result.TargetType = (string)items["Target Type"];
            result.UniqueIdentifier = (string)items["Unique Identifier"];
            result.ITunesVersion = (string)items["iTunes Version"];

            return result;
        }
    }
}
