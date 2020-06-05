using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EbookConverter.Configs;
using FB2Library;

namespace EbookConverter.Converters.Fb2 {
    public class Fb2Converter : ConverterBase {
        private readonly HtmlPatternsConfig _htmlPatternsConfig;

        public Fb2Converter(HtmlPatternsConfig htmlPatternsConfig) {
            _htmlPatternsConfig = htmlPatternsConfig;
        }
        
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".fb2", StringComparison.InvariantCultureIgnoreCase);
        }

        private string CreateCover(FB2File file, string directoryPath) {
            var (key, _) = file.Images.FirstOrDefault(i => i.Key.Contains("cover"));
            if (key == null) {
                return string.Empty;
            }
            
            var coverPattern = File.ReadAllText(_htmlPatternsConfig.CoverPath).Replace("{cover}", key);
            
            var covertPath = Path.Combine(directoryPath, "cover.html");
            File.WriteAllText(covertPath, coverPattern, Encoding.UTF8);
            return covertPath;

        }

        private static void SaveImages(FB2File file, string directoryPath) {
            foreach (var (key, value) in file.Images) {
                File.WriteAllBytes(Path.Combine(directoryPath, key), value.BinaryData);
            }
        }

        private string CreateContent(FB2File file, string directoryPath) {
            var sections = Fb2ToLinesConverter.Convert(file);
            var sb = new StringBuilder();
            var pattern = File.ReadAllText(_htmlPatternsConfig.ContentPath);
            pattern = pattern.Replace("{title}", file.TitleInfo.BookTitle.ToString());
            foreach (var line in sections) {
                sb.AppendLine(line.ToString());
            }

            var contentPath = Path.Combine(directoryPath, "content.html");
            File.WriteAllText(contentPath, pattern.Replace("{content}", sb.ToString()), Encoding.UTF8);

            return contentPath;
        }

        protected override bool ConvertInternal(string tempPath, string sourcePath, string destinationPath, string wkArgs) {
            var file = new FB2Reader().ReadAsync(File.ReadAllText(sourcePath)).Result;
            
            var coverPath = CreateCover(file, tempPath);
            var contentPath = CreateContent(file, tempPath);

            SaveImages(file, tempPath);
            
            var generatePdf = GeneratePdf(coverPath, contentPath, destinationPath, wkArgs);

            return generatePdf;
        }
    }
}
