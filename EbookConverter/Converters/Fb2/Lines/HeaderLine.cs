namespace EbookConverter.Converters.Fb2.Lines {
    public class HeaderLine : ILine {
        public string Text;
        public int HeaderLevel;
        public string Id;
        
        public string ToHtml() {
            return $"<h{HeaderLevel} id=\"{Id}\">{Text}</h{HeaderLevel}>";
        }
    }
}
