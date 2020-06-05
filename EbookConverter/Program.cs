using CommandLine;
using EbookConverter.Configs;
using EbookConverter.IoC;
using EbookConverter.Logic;
using Ninject;

namespace EbookConverter {
    public class Program {
        private static readonly StandardKernel _kernel = new StandardKernel(new EpubConverterNinject());

        private static void Main(string[] args) {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => _kernel.Get<Processor>().ProcessDirectory(options.Source, options.Destination, options.Pattern, options.Wk))
                .WithNotParsed(options => {});
        }
    }
}
