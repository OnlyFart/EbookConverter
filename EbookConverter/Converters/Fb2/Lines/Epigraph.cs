using System.Collections.Generic;
using System.Text;
using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    /// <summary>
    /// Эпиграф
    /// </summary>
    public class Epigraph : ILine {
        public readonly List<ILine> Texts = new List<ILine>();
        public readonly List<ILine> Authors  = new List<ILine>();

        public string ToHtml() {
            var sb = new StringBuilder();
            
            // Основной контент эпиграфм
            foreach (var text in Texts) {
                sb.AppendLine(text.ToHtml());
            }
            
            // Авторы эпиграфа
            foreach (var text in Authors) {
                sb.AppendLine(text.ToHtml().ToHtmlTag("cite"));
            }
            
            // Оборачивание основного контента и авторов в тег blockquote
            return sb
                .ToString()
                .ToHtmlTag("blockquote");
        }
    }
}
