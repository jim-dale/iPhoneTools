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

            Directory.CreateDirectory(opts.Output);

            _ = _appContext
                .SetBackupMetadataInputPaths(opts.Input)
                .CopyInfoPropertyListFileToOutput(opts.Output, opts.Overwrite)
                .CopyStatusPropertyListFileToOutput(opts.Output, opts.Overwrite)
                .CopyManifestPropertyListFileToOutput(opts.Output, opts.Overwrite)
                .LoadBackupMetadata()
                .UnwrapClassKeysFromManifestKeyBag(opts.Password)
                .SetVersionsFromMetadata()
                .SetManifestEntriesFileInputPath(opts.Input)
                .CopyManifestEntriesFileToOutputAsPlainText(opts.Output, opts.Overwrite)
                .AddManifestEntries()
                .CopyManifestEntryFilesToOutputAsPlainText(opts.Input, opts.Output, opts.Overwrite);

            _logger.LogInformation("Completed {Command}", nameof(DecryptCommand));

            return 0;
        }
    }
}
