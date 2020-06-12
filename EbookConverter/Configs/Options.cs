using CommandLine;

namespace EbookConverter.Configs {
    public class Options {
        [Option('s', "source", Required = true, HelpText = "Source directory for read ebook files.")]
        public string Source { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination directory for save pdf files")]
        public string Destination { get; set; }

        [Option('p', "pattern", Default = "*", Required = false, HelpText = "Pattern for search in source directory (*.epub, *.fb2)")]
        public string Pattern { get; set; }
        
        [Option("wk", Required = false, HelpText = "Wkhtmltopdf options. See https://wkhtmltopdf.org/usage/wkhtmltopdf.txt for details")]
        public string Wk { get; set; }
    }
}
