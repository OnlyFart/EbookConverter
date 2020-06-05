using System;
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
            var (key, value) = file.Images.FirstOrDefault(i => i.Key.Contains("cover"));
            if (key == null) {
                return string.Empty;
            }

            File.WriteAllBytes(Path.Combine(directoryPath, key), value.BinaryData);
            var coverPattern = File.ReadAllText(_htmlPatternsConfig.CoverPath).Replace("{cover}", key);
            
            var covertPath = Path.Combine(directoryPath, "cover.html");
            File.WriteAllText(covertPath, coverPattern, Encoding.UTF8);
            return covertPath;

        }

        private string CreateContent(FB2File file, string directoryPath) {
            var sections = Fb2ToLinesConverter.Convert(file);
            var sb = new StringBuilder();
            var pattern = File.ReadAllText(_htmlPatternsConfig.ContentPath);
            foreach (var line in sections) {
                switch (line) {
                    case HeaderLine header: {
                        if (header.HeaderLevel == 1) {
                            pattern = pattern.Replace("{title}", header.ToString());
                        }

                        sb.AppendLine($"<h{header.HeaderLevel} id=\"{header.Id}\">{header}</h{header.HeaderLevel}>");
                        break;
                    }
                    case SubTitle _:
                        sb.AppendLine($"<h3>{line}</h3>");
                        break;
                    case TextLine _:
                        sb.AppendLine($"<p>{line}</p>");
                        break;
                    case ImageLine image: {
                        File.WriteAllBytes(Path.Combine(directoryPath, image.Key), image.Data);
                        sb.AppendLine($"<div style=\"text-align: center\"><img alt=\"{image.Key}\" src=\"{image.Key}\" /></div>");
                        break;
                    }
                }
            }

            var contentPath = Path.Combine(directoryPath, "content.html");
            File.WriteAllText(contentPath, pattern.Replace("{content}", sb.ToString()), Encoding.UTF8);

            return contentPath;
        }

        public override bool Convert(string sourcePath, string destinationPath, string wkArgs) {
            var file = new FB2Reader().ReadAsync(File.ReadAllText(sourcePath)).Result;
            var directoryPath = GetTemporaryDirectory();
            
            var coverPath = CreateCover(file, directoryPath);
            var contentPath = CreateContent(file, directoryPath);
            
            var generatePdf = GeneratePdf(coverPath, contentPath, destinationPath, wkArgs);
            
            Directory.Delete(directoryPath, true);
            
            return generatePdf;
        }
    }
}
