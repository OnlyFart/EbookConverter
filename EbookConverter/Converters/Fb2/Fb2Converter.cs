using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private async Task<string> CreateCover(FB2File file, string temp) {
            var coverImagePath = string.Empty;
            
            var coverImage = file.TitleInfo.Cover?.CoverpageImages.FirstOrDefault();
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
            await File.WriteAllTextAsync(covertPath, (await File.ReadAllTextAsync(_htmlPatternsConfig.CoverPath)).Replace("{cover}", coverImagePath), Encoding.UTF8);
            return covertPath;

        }
        
        /// <summary>
        /// Сохранение все картинок на диск
        /// </summary>
        /// <param name="file"></param>
        /// <param name="temp"></param>
        private static async Task SaveImages(FB2File file, string temp) {
            foreach (var (key, value) in file.Images) {
                await File.WriteAllBytesAsync(Path.Combine(temp, key), value.BinaryData);
            }
        }
        
        /// <summary>
        /// Создание файла с основным контентом
        /// </summary>
        /// <param name="file">Файл fb2</param>
        /// <param name="temp">Директория для сохранения</param>
        /// <returns></returns>
        private async Task<string> CreateContent(FB2File file, string temp) {
            var sb = new StringBuilder();
            foreach (var line in Fb2ToLinesConverter.Convert(file)) {
                sb.AppendLine(line.ToHtml());
            }
            
            var pattern = (await File.ReadAllTextAsync(_htmlPatternsConfig.ContentPath)).Replace("{title}", file.TitleInfo.BookTitle.ToString());
            var contentPath = Path.Combine(temp, "content.html");
            await File.WriteAllTextAsync(contentPath, pattern.Replace("{content}", sb.ToString()), Encoding.UTF8);

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
        protected override async Task<bool> ConvertInternal(string temp, string source, string destination, string wkArgs) {
            var file = await _reader.ReadAsync(await File.ReadAllTextAsync(source));
            
            var coverPath = await CreateCover(file, temp);
            var contentPath = await CreateContent(file, temp);

            await SaveImages(file, temp);

            return await GeneratePdf(coverPath, contentPath, destination, wkArgs);
        }
    }
}
