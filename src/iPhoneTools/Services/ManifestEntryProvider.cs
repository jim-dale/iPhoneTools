using System;
using System.Collections.Generic;

namespace iPhoneTools
{
    public class ManifestEntryProvider
    {
        private string _path;
        private bool _isEncryptedBackup;
        private Func<IEnumerable<ManifestEntry>> _getItems;

        public ManifestEntryProvider()
        {
        }

        public ManifestEntryProvider FromMbdbFile(string path, bool isEncryptedBackup)
        {
            _path = path;
            _isEncryptedBackup = isEncryptedBackup;
            _getItems = GetMbdbFiles;

            return this;
        }

        public ManifestEntryProvider FromManifestDbFile(string path, bool isEncryptedBackup)
        {
            _path = path;
            _isEncryptedBackup = isEncryptedBackup;
            _getItems = GetManifestDbFiles;

            return this;
        }

        public IEnumerable<ManifestEntry> GetAllFiles()
        {
            var items = _getItems.Invoke();
            foreach (var item in items)
            {
                yield return item;
            }
        }

        private IEnumerable<ManifestEntry> GetMbdbFiles()
        {
            var manifest = new MbdbReader()
                .LoadFrom(_path);

            foreach (var item in manifest.Items)
            {
                var entryType = CommonHelpers.GetManifestEntryTypeFromMode(item.Mode);
                if (entryType == ManifestEntryType.File)
                {
                    var result = new ManifestEntry
                    {
                        FileId = item.GetSha1HashAsHexString(),
                        Domain = item.Domain,
                        RelativePath = item.RelativePath,
                        FileType = entryType,
                    };

                    if (_isEncryptedBackup)
                    {
                        result.ProtectionClass = (ProtectionClass)item.Flags;
                        result.WrappedKey = item.WrappedKey;
                    }

                    yield return result;
                }
            }
        }

        private IEnumerable<ManifestEntry> GetManifestDbFiles()
        {
            using (var repository = new ManifestRepository(SqliteRepository.GetConnectionString(_path)))
            {
                var items = repository.GetAllItems();

                foreach (var item in items)
                {
                    var propertyList = new BinaryPropertyListReader()
                        .LoadFrom(item.Properties);

                    if (propertyList != null && propertyList is IReadOnlyDictionary<string, object>)
                    {
                        dynamic properties = propertyList;

                        var mode = (int)properties["$objects"][1]["Mode"];
                        var entryType = CommonHelpers.GetManifestEntryTypeFromMode(mode);
                        if (entryType == ManifestEntryType.File)
                        {
                            var result = new ManifestEntry
                            {
                                FileId = item.FileID,
                                Domain = item.Domain,
                                RelativePath = item.RelativePath,
                                FileType = entryType,
                            };

                            if (_isEncryptedBackup)
                            {
                                var protectionClass = (ProtectionClass)properties["$objects"][1]["ProtectionClass"];
                                var index = (int)properties["$objects"][1]["EncryptionKey"];
                                var data = (byte[])properties["$objects"][index]["NS.data"];

                                result.ProtectionClass = protectionClass;
                                result.WrappedKey = WrappedKeyReader.Read(data);
                            }

                            yield return result;
                        }
                    }
                }
            }
        }
    }
}
