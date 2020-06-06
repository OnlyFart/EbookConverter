using System.IO;
using System.Linq;
using System.Text;
using EbookConverter.Configs;
using FB2Library;

namespace EbookConverter.Converters.Fb2 {
    public class Fb2Converter : ConverterBase {
        private readonly IFB2Reader _reader;
        private readonly HtmlPatternsConfig _htmlPatternsConfig;

        public Fb2Converter(IFB2Reader reader, HtmlPatternsConfig htmlPatternsConfig) : base(".fb2") {
            _reader = reader;
            _htmlPatternsConfig = htmlPatternsConfig;
        }
        
        private string CreateCover(FB2File file, string directoryPath) {
            var coverImagePath = string.Empty;
            
            var coverImage = file.TitleInfo.Cover.CoverpageImages.FirstOrDefault();
            if (coverImage != null) {
                coverImagePath = coverImage.HRef.Replace("#", string.Empty);
            }

            if (string.IsNullOrEmpty(coverImagePath)) {
                var (key, _) = file.Images.FirstOrDefault(i => i.Key.Contains("cover"));
                if (key == null) {
                    return string.Empty;
                }

                coverImagePath = key;
            }

            var covertPath = Path.Combine(directoryPath, "cover.html");
            File.WriteAllText(covertPath, File.ReadAllText(_htmlPatternsConfig.CoverPath).Replace("{cover}", coverImagePath), Encoding.UTF8);
            return covertPath;

        }

        private static void SaveImages(FB2File file, string directoryPath) {
            foreach (var (key, value) in file.Images) {
                File.WriteAllBytes(Path.Combine(directoryPath, key), value.BinaryData);
            }
        }

        private string CreateContent(FB2File file, string directoryPath) {
            var sb = new StringBuilder();
            foreach (var line in Fb2ToLinesConverter.Convert(file)) {
                sb.AppendLine(line.ToHtml());
            }
            
            var pattern = File.ReadAllText(_htmlPatternsConfig.ContentPath).Replace("{title}", file.TitleInfo.BookTitle.ToString());
            var contentPath = Path.Combine(directoryPath, "content.html");
            File.WriteAllText(contentPath, pattern.Replace("{content}", sb.ToString()), Encoding.UTF8);

            return contentPath;
        }

        protected override bool ConvertInternal(string temp, string source, string destination, string wkArgs) {
            var file = _reader.ReadAsync(File.ReadAllText(source)).Result;
            
            var coverPath = CreateCover(file, temp);
            var contentPath = CreateContent(file, temp);

            SaveImages(file, temp);

            return GeneratePdf(coverPath, contentPath, destination, wkArgs);
        }
    }
}
