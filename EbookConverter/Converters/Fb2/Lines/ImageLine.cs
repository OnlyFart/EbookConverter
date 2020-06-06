namespace EbookConverter.Converters.Fb2.Lines {
    public class ImageLine : ILine {
        public byte[] Data;
        public string Key;
        public string Id;
        
        public string ToHtml() {
            return $"<div style=\"text-align: center\"><img id=\"{Id}\" alt=\"{Key}\" src=\"{Key}\" /></div>";
        }
    }
}
