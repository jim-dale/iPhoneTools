using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class ContactCommand : ICommand<ContactOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<ContactCommand> _logger;

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

            //    var path = Path.Combine(input, "contacts.db");

            //    List<ContactDbEntry> items = default;

            //    using (var repository = new ContactRepository(SqliteRepository.GetConnectionString(path)))
            //    {
            //        items = repository.GetAllItems().ToList();
            //    }

            _logger.LogInformation("Completed {Command}", nameof(ContactCommand));
            return 0;
        }
    }
}
