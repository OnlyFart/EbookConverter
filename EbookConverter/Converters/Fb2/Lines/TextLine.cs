using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    public class TextLine : ILine {
        public string Text;
        
        public string ToHtml() {
            // Просто оборачиваем в тег p
            return Text.ToHtmlTag("p");
        }
    }
}
