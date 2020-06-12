using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using EbookConverter.Configs;
using VersOne.Epub;

namespace EbookConverter.Converters.Epub {
    public class EpubConverter : ConverterBase {
        public EpubConverter(WkhtmltopdfConfig wkhtmltopdfConfig) : base(".epub", wkhtmltopdfConfig) { }
        
        protected override bool ConvertInternal(string temp, string source, string destination, string wkArgs) {
            ZipFile.ExtractToDirectory(source, temp, true);
            var readBook = EpubReader.ReadBook(source);
            
            if (readBook.ReadingOrder.Count == 0) {
                throw new Exception("No pages for convert");
            }
            
            var parts = readBook.ReadingOrder
                .Where(t => !string.Equals(Path.GetFileName(t.FileName), "TOC.xhtml", StringComparison.InvariantCultureIgnoreCase))
                .Select(t => Path.Combine(temp, readBook.Schema.ContentDirectoryPath, t.FileName)).ToList();

            if (parts.Count == 0) {
                return false;
            }

            return GeneratePdf(parts[0], parts.Skip(1).ToList(), destination, wkArgs);
        }
    }
}
