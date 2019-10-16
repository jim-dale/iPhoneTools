using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    public class ContactCommand : ICommand<ContactOptions>
    {
        private readonly ILogger<ContactCommand> _logger;

        public ContactCommand(ILogger<ContactCommand> logger)
        {
            _logger = logger;
        }

        public int Run(ContactOptions opts)
        {
            int result = default;
            _logger.LogInformation("Starting {Command}", nameof(ContactCommand));

            //    var path = Path.Combine(input, "contacts.db");

            //    List<ContactDbEntry> items = default;

            //    using (var repository = new ContactRepository(SqliteRepository.GetConnectionString(path)))
            //    {
            //        items = repository.GetAllItems().ToList();
            //    }

            _logger.LogInformation("Completed {Command}", nameof(ContactCommand));
            return result;
        }
    }
}
