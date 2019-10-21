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
            _getItems = GetMbdbFileEntries;

            return this;
        }

        public ManifestEntryProvider FromManifestDbFile(string path, bool isEncryptedBackup)
        {
            _path = path;
            _isEncryptedBackup = isEncryptedBackup;
            _getItems = GetManifestDbFileEntries;

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

        private IEnumerable<ManifestEntry> GetMbdbFileEntries()
        {
            var manifest = new MbdbReader()
                .LoadFrom(_path);

            foreach (var item in manifest.Items)
            {
                var result = item.ConvertToManifestEntry(ManifestEntryType.File, _isEncryptedBackup);
                if (result != null)
                {
                    yield return result;
                }
            }
        }

        private IEnumerable<ManifestEntry> GetManifestDbFileEntries()
        {
            using (var repository = new ManifestRepository(SqliteRepository.GetConnectionString(_path)))
            {
                var items = repository.GetAllItems();

                foreach (var item in items)
                {
                    var result = item.ConvertToManifestEntry(ManifestEntryType.File, _isEncryptedBackup);
                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}
