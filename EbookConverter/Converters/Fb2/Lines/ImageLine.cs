using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    /// <summary>
    /// Картинка
    /// </summary>
    public class ImageLine : ILine {
        public string Key;
        public string Id;
        
        public string ToHtml() {
            return HtmlExtensions
                .SelfCloseHtmlTag("img", "id", Id, "alt", Key, "src", Key) // Создание тега с картинкой
                .ToHtmlTag("div"); // Оборачивание картинки в div
        }
    }
}
