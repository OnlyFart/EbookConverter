using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    public class HeaderLine : ILine {
        public string Text;
        public int HeaderLevel;
        public string Id;
        
        public string ToHtml() {
            return Text.ToHtmlTag($"h{HeaderLevel}", "id", Id);
        }
    }
}
