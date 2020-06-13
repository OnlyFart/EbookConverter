using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    /// <summary>
    /// Заголовок
    /// </summary>
    public class HeaderLine : ILine {
        public string Text;
        public int HeaderLevel;
        public string Id;
        
        public string ToHtml() {
            // Оборачивание текста заголовка в тег h соорветствующего уровня
            return Text.ToHtmlTag($"h{HeaderLevel}", "id", Id);
        }
    }
}
