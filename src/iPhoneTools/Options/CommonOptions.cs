using CommandLine;

namespace iPhoneTools
{
    public class CommonOptions
    {
        [Option(Default = false, HelpText = "Prints all messages to standard output")]
        public bool Verbose { get; set; }
    }
}
