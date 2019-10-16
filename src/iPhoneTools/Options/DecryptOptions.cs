using CommandLine;

namespace iPhoneTools
{
    [Verb("decrypt", HelpText = "Decrypt an iPhone backup")]
    public class DecryptOptions : CommonOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input folder containing the encrypted iPhone backup")]
        public string Input { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder for decrypted files")]
        public string Output { get; set; }

        [Option('p', "password", Required = true, HelpText = "Password for the iPhone backup")]
        public string Password { get; set; }
        [Option("overwrite", HelpText = "Overwrite files in the output folder")]
        public bool Overwrite { get; set; }
    }
}
