using System.Collections.Generic;
using System.Text;

namespace EbookConverter.Converters.Fb2 {
    public interface ILine {
        
    }
    
    public class HeaderLine : ILine {
        public string Text { get; set; }
        public int HeaderLevel { get; set; }
        public string Id { get; set; }
        public override string ToString() {
            return $"<h{HeaderLevel} id=\"{Id}\">{Text}</h{HeaderLevel}>";
        }
    }

    public class ImageLine : ILine  {
        public byte[] Data { get; set; }
        public string Key { get; set; }
        public string Id { get; set; }
        public override string ToString() {
            return $"<div style=\"text-align: center\"><img id=\"{Id}\" alt=\"{Key}\" src=\"{Key}\" /></div>";
        }
    }

    public class Epigraph : ILine {
        public List<ILine> Texts { get; set; } = new List<ILine>();
        public List<ILine> Authors { get; set; } = new List<ILine>();
        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("<blockquote>");
            foreach (var text in Texts) {
                sb.AppendLine(text.ToString());
            }

            foreach (var text in Authors) {
                sb.AppendLine("<cite>" + text + "</cite>");
            }

            sb.AppendLine("</blockquote>");
            return sb.ToString();
        }
    }

    public class TextLine : ILine  {
        public string Text { get; set; }
        public override string ToString() {
            return $"<p>{Text}</p>";
        }
    }

    public class NoteLine : ILine {
        public List<ILine> Texts  { get; set; } = new List<ILine>();
        public string Id;
        public List<ILine> Titles  { get; set; } = new List<ILine>();

        public override string ToString() {
            var pattern = "<p id=\"{id}\"><strong><a href=\"#{id}_backlink\">[{title}]</strong></a> {text}</p>";
            pattern = pattern.Replace("{id}", Id);
            foreach (var title in Titles) {
                pattern = pattern.Replace("{title}", ((HeaderLine) title).Text.Trim());
            }
            
            foreach (var title in Texts) {
                pattern = pattern.Replace("{text}", ((TextLine) title).Text);
            }

            return pattern;
        }
    }
}
