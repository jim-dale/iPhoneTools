using CommandLine;

namespace iPhoneTools
{
    [Verb("manifest", HelpText = "Show manifest")]
    public class ManifestOptions : CommonOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input folder containing the iPhone backup")]
        public string InputFolder { get; set; }
    }
}
