using System.IO;
using Microsoft.Extensions.Logging;

namespace iPhoneTools
{
    /// <summary>
    /// </summary>
    public class DecryptCommand : ICommand<DecryptOptions>
    {
        private readonly AppContext _appContext;
        private readonly ILogger<DecryptCommand> _logger;

        public DecryptCommand(AppContext appContext, ILogger<DecryptCommand> logger)
        {
            _appContext = appContext;
            _logger = logger;
        }

        public int Run(DecryptOptions opts)
        {
            _logger.LogInformation("Starting {Command}", nameof(DecryptCommand));

            Directory.CreateDirectory(opts.OutputFolder);

            _ = _appContext
                .SetBackupMetadataInputPaths(opts.InputFolder)
                .CopyInfoPropertyListFileToOutput(opts.OutputFolder, opts.Overwrite)
                .CopyStatusPropertyListFileToOutput(opts.OutputFolder, opts.Overwrite)
                .CopyManifestPropertyListFileToOutput(opts.OutputFolder, opts.Overwrite)
                .LoadBackupMetadata()
                .SetVersionsFromMetadata()
                .SetClassKeysFromManifestKeyBag(opts.Password)
                .SetManifestKey()
                .SetManifestEntriesFileInputPath(opts.InputFolder)
                .CopyManifestEntriesFileToOutputAsPlainText(opts.OutputFolder, opts.Overwrite)
                .AddManifestEntries()
                .CopyManifestEntryFilesToOutputAsPlainText(opts.InputFolder, opts.OutputFolder, opts.Overwrite);

            _logger.LogInformation("Completed {Command}", nameof(DecryptCommand));

            return 0;
        }
    }
}
