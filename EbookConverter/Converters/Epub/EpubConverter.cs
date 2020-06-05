using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using VersOne.Epub;

namespace EbookConverter.Converters.Epub {
    public class EpubConverter : ConverterBase {
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".epub", StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Convert(string sourcePath, string destinationPath, string wkArgs) {
            var temp = GetTemporaryDirectory();
            ZipFile.ExtractToDirectory(sourcePath, temp, true);
            var readBook = EpubReader.ReadBook(sourcePath);
            
            if (readBook.ReadingOrder.Count == 0) {
                throw new Exception("No pages for convert");
            }
            
            var parts = readBook.ReadingOrder
                .Where(t => !string.Equals(Path.GetFileName(t.FileName), "TOC.xhtml", StringComparison.InvariantCultureIgnoreCase))
                .Select(t => "\"" + Path.Combine(temp, readBook.Schema.ContentDirectoryPath, t.FileName) + "\"").ToList();

            if (parts.Count == 0) {
                return false;
            }

            var generatePdf = GeneratePdf(parts[0], parts.Skip(1).ToList(), destinationPath, wkArgs);
            
            Directory.Delete(temp, true);
            
            return generatePdf;
        }
    }
}
