using System;
using System.Collections.Generic;

namespace iPhoneTools
{
    public static class BackupManifestPropertiesExtensions
    {
        public static BackupManifestProperties AddPropertyList(this BackupManifestProperties result, IReadOnlyDictionary<string, object> items)
        {
            result.Version = (string)items["Version"];
            result.Date = (DateTimeOffset)items["Date"];
            result.SystemDomainsVersion = (string)items["SystemDomainsVersion"];
            result.WasPasscodeSet = (bool)items["WasPasscodeSet"];
            result.IsEncrypted = (bool)items["IsEncrypted"];

            return result;
        }
    }
}
