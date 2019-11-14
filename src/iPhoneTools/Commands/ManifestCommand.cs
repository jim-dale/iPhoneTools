using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    /// <summary>
    /// </summary>
    public class ManifestCommand : ICommand<ManifestOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<ManifestCommand> _logger;

        public ManifestCommand(AppContext appContext, ILogger<ManifestCommand> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public int Run(ManifestOptions opts)
        {
            _logger.LogInformation("Starting {Command}", nameof(ManifestCommand));

            _ = _appContext
                .SetBackupMetadataInputPaths(opts.InputFolder)
                .LoadBackupMetadata()
                .SetVersionsFromMetadata()
                .SetManifestEntriesFileInputPath(opts.InputFolder)
                .AddManifestEntries();

            foreach (var item in _appContext.ManifestEntries)
            {
                item.ConsoleWrite();
            }

            _logger.LogInformation("Completed {Command}", nameof(ManifestCommand));

            return 0;
        }
    }
}
