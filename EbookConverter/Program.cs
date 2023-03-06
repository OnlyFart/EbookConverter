using System.Threading.Tasks;
using CommandLine;
using EbookConverter.Configs;
using EbookConverter.IoC;
using EbookConverter.Logic;
using Ninject;

namespace EbookConverter {
    public class Program {
        private static readonly StandardKernel Kernel = new(new EpubConverterNinject());

        private static async Task Main(string[] args) {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async options => await Kernel.Get<Processor>().ProcessDirectory(options.Source, options.Destination, options.Pattern, options.Wk));
        }
    }
}
