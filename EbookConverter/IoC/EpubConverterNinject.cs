using EbookConverter.Configs;
using EbookConverter.Converters;
using EbookConverter.Converters.Epub;
using EbookConverter.Converters.Fb2;
using FB2Library;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;

namespace EbookConverter.IoC {
    public class EpubConverterNinject : NinjectModule{
        public override void Load() {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            
            var htmlPatternsConfig = config.GetSection("HtmlPatternsConfig").Get<HtmlPatternsConfig>();

            Bind<HtmlPatternsConfig>().ToConstant(htmlPatternsConfig);
            
            Bind<IFB2Reader>().To<FB2Reader>();
            Bind<ConverterBase>().To<EpubConverter>();
            Bind<ConverterBase>().To<Fb2Converter>();
        }
    }
}
