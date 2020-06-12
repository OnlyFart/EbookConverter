using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    public class ImageLine : ILine {
        public string Key;
        public string Id;
        
        public string ToHtml() {
            return HtmlExtensions.SelfCloseHtmlTag("img", "id", Id, "alt", Key, "src", Key).ToHtmlTag("div");
        }
    }
}
