using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class ContactCommand : ICommand<ContactOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<ContactCommand> _logger;

        private BackupFileProvider _backupFileProvider;

        public ContactCommand(AppContext appContext, ILogger<ContactCommand> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public int Run(ContactOptions opts)
        {
            _logger.LogInformation("Starting {Command}", nameof(ContactCommand));

            var appContext = _appContext
                .SetBackupMetadataInputPaths(opts.InputFolder)
                .LoadBackupMetadata()
                .SetVersionsFromMetadata();

            _backupFileProvider = new BackupFileProvider(opts.InputFolder, appContext.ManifestVersion.Major);

            var input = _backupFileProvider.GetPath(KnownDomains.HomeDomain, KnownFiles.AddressBook);

            List<ContactDbEntry> items = default;

            _logger.LogInformation("Opening contacts database '{Source}'", input);
            using (var repository = new ContactRepository(SqliteRepository.GetConnectionString(input)))
            {
                items = repository.GetAllItems().ToList();

                foreach (var item in items)
                {
                    ContactAsVCard.Save(item);
                    //break;
                }
            }

            _logger.LogInformation("Completed {Command}", nameof(ContactCommand));
            return 0;
        }
    }
}
