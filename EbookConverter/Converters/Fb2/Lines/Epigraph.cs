using System.Collections.Generic;
using System.Text;
using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    public class Epigraph : ILine {
        public readonly List<ILine> Texts = new List<ILine>();
        public readonly List<ILine> Authors  = new List<ILine>();

        public string ToHtml() {
            var sb = new StringBuilder();
            foreach (var text in Texts) {
                sb.AppendLine(text.ToHtml());
            }

            foreach (var text in Authors) {
                sb.AppendLine(text.ToHtml().ToHtmlTag("cite"));
            }
            return sb.ToString().ToHtmlTag("blockquote");
        }
    }
}
