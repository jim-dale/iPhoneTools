﻿using CommandLine;

namespace iPhoneTools
{
    [Verb("contact", HelpText = "Export contacts")]
    public class ContactOptions : CommonOptions
    {
        [Option('i', "input", Required = true, HelpText = "Input folder containing the unencrypted iPhone backup")]
        public string InputFolder { get; set; }

        [Option('o', "output", Required = true, HelpText = "Output folder")]
        public string OutputFolder { get; set; }
    }
}
