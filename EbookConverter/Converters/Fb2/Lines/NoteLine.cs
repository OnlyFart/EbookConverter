using System.Collections.Generic;
using System.Linq;
using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    public class NoteLine : ILine {
        public readonly List<ILine> Texts  = new List<ILine>();
        public readonly List<ILine> Titles = new List<ILine>();
        public string Id;
        
        public string ToHtml() {
            var pattern = "[{title}]"
                .ToHtmlTag("a", "href", "#{id}_backlink")
                .ToHtmlTag("strong")
                .AppendThroughWhitespace("{text}")
                .ToHtmlTag("p", "id", "{id}");
            
            pattern = Titles.Aggregate(pattern.Replace("{id}", Id), (current, title) => current.Replace("{title}", ((HeaderLine) title).Text.Trim()));

            return Texts.Aggregate(pattern, (current, title) => current.Replace("{text}", ((TextLine) title).Text));
        }
    }
}
