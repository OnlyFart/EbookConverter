using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using EbookConverter.Configs;
using EpubSharp;

namespace EbookConverter.Converters.Epub {
    public class EpubConverter : ConverterBase {
        public EpubConverter(WkhtmltopdfConfig wkhtmltopdfConfig) : base(".epub", wkhtmltopdfConfig) { }
        
        /// <summary>
        /// Конвертация файла из epub в pdf
        /// </summary>
        /// <param name="temp">Путь к временной папке для хранения промежуточных данных. После выхода из функции папка будет удалена</param>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected override async Task<bool> ConvertInternal(string temp, string source, string destination, string wkArgs) {
            ZipFile.ExtractToDirectory(source, temp, true);
            var readBook = EpubReader.Read(source);
            
            if (readBook.TableOfContents.Count == 0) {
                throw new Exception("No pages for convert");
            }
            
            var parts = readBook.TableOfContents
                .Where(t => !string.Equals(Path.GetFileName(t.AbsolutePath), "TOC.xhtml", StringComparison.InvariantCultureIgnoreCase))
                .Select(t => Path.Combine(temp, t.AbsolutePath)).ToList();

            return parts.Count != 0 && await GeneratePdf(parts[0], parts.Skip(1).ToList(), destination, wkArgs);
        }
    }
}
