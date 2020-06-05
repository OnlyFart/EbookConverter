namespace EbookConverter.Converters.Fb2 {
    public interface ILine {
        
    }
    
    public class HeaderLine : ILine  {
        public string Text { get; set; }
        public int HeaderLevel { get; set; }
        public string Id { get; set; }
        public override string ToString() {
            return Text;
        }
    }
    
    public class SubTitle : ILine  {
        public string Text { get; set; }
        public override string ToString() {
            return Text;
        }
    }

    public class ImageLine : ILine
    {
        public byte[] Data { get; set; }
        public string Key { get; set; }
    }

    public class TextLine : ILine
    {
        public string Text { get; set; }
        public override string ToString() {
            return Text;
        }
    }
}
