using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class AppContext
    {
        internal ILogger<AppContext> Logger { get; }
        internal KeyStore KeyStore { get; }

        public string InfoPropertiesPath { get; set; }
        public IReadOnlyDictionary<string,object> InfoPropertyList { get; internal set; }
        public BackupInfoProperties InfoProperties { get; set; }

        public string StatusPropertiesPath { get; set; }
        public IReadOnlyDictionary<string, object> StatusPropertyList { get; internal set; }
        public BackupStatusProperties StatusProperties { get; set; }

        public string ManifestPropertiesPath { get; set; }
        public IReadOnlyDictionary<string, object> ManifestPropertyList { get; internal set; }
        public BackupManifestProperties ManifestProperties { get; internal set; }

        public string ManifestEntriesPath { get; set; }

        public Version ITunesVersion { get; internal set; }
        public Version ProductVersion { get; internal set; }
        public Version StatusVersion { get; internal set; }
        public Version ManifestVersion { get; internal set; }

        public IReadOnlyList<ManifestEntry> ManifestEntries { get; internal set; }

        public AppContext(ILogger<AppContext> logger, KeyStore keyStore)
        {
            Logger = logger;
            KeyStore = keyStore;
        }
    }
}
