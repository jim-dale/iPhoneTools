using CommandLine;

namespace iPhoneTools
{
    [Verb("sms", HelpText = "Export messages as HTML")]
    public class MessageOptions : CommonOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input folder containing the unencrypted iPhone backup")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder")]
        public string Output { get; set; }
    }
}
