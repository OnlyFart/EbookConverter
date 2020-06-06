namespace EbookConverter.Converters.Fb2.Lines {
    public class TextLine : ILine {
        public string Text;
        
        public string ToHtml() {
            return $"<p>{Text}</p>";
        }
    }
}
