using System.Collections.Generic;

namespace EbookConverter.Converters.Fb2.Lines {
    public class NoteLine : ILine {
        public readonly List<ILine> Texts  = new List<ILine>();
        public readonly List<ILine> Titles = new List<ILine>();
        public string Id;
        
        public string ToHtml() {
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
