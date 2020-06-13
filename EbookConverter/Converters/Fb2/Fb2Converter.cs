using System.IO;
using System.Linq;
using System.Text;
using EbookConverter.Configs;
using FB2Library;

namespace EbookConverter.Converters.Fb2 {
    public class Fb2Converter : ConverterBase {
        private readonly IFB2Reader _reader;
        private readonly HtmlPatternsConfig _htmlPatternsConfig;

        public Fb2Converter(IFB2Reader reader, HtmlPatternsConfig htmlPatternsConfig, WkhtmltopdfConfig wkhtmltopdfConfig) : base(".fb2", wkhtmltopdfConfig) {
            _reader = reader;
            _htmlPatternsConfig = htmlPatternsConfig;
        }
        
        /// <summary>
        /// Создание файла обложки
        /// </summary>
        /// <param name="file">Файл fb2</param>
        /// <param name="temp">Директория для сохранения</param>
        /// <returns></returns>
        private string CreateCover(FB2File file, string temp) {
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

            var covertPath = Path.Combine(temp, "cover.html");
            File.WriteAllText(covertPath, File.ReadAllText(_htmlPatternsConfig.CoverPath).Replace("{cover}", coverImagePath), Encoding.UTF8);
            return covertPath;

        }
        
        /// <summary>
        /// Сохрание все картинок на диск
        /// </summary>
        /// <param name="file"></param>
        /// <param name="temp"></param>
        private static void SaveImages(FB2File file, string temp) {
            foreach (var (key, value) in file.Images) {
                File.WriteAllBytes(Path.Combine(temp, key), value.BinaryData);
            }
        }
        
        /// <summary>
        /// Создание файла с основным контентом
        /// </summary>
        /// <param name="file">Файл fb2</param>
        /// <param name="temp">Директория для сохранения</param>
        /// <returns></returns>
        private string CreateContent(FB2File file, string temp) {
            var sb = new StringBuilder();
            foreach (var line in Fb2ToLinesConverter.Convert(file)) {
                sb.AppendLine(line.ToHtml());
            }
            
            var pattern = File.ReadAllText(_htmlPatternsConfig.ContentPath).Replace("{title}", file.TitleInfo.BookTitle.ToString());
            var contentPath = Path.Combine(temp, "content.html");
            File.WriteAllText(contentPath, pattern.Replace("{content}", sb.ToString()), Encoding.UTF8);

            return contentPath;
        }
        
        /// <summary>
        /// Конвертация файла из fb2 в pdf
        /// </summary>
        /// <param name="temp">Путь к временной папке для хранения промежуточных данных. После выхода из функции папка будет удалена</param>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected override bool ConvertInternal(string temp, string source, string destination, string wkArgs) {
            var file = _reader.ReadAsync(File.ReadAllText(source)).Result;
            
            var coverPath = CreateCover(file, temp);
            var contentPath = CreateContent(file, temp);

            SaveImages(file, temp);

            return GeneratePdf(coverPath, contentPath, destination, wkArgs);
        }
    }
}
