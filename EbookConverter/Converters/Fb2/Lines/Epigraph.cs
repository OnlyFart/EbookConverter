using System.Collections.Generic;
using System.Text;

namespace EbookConverter.Converters.Fb2.Lines {
    public class Epigraph : ILine {
        public readonly List<ILine> Texts = new List<ILine>();
        public readonly List<ILine> Authors  = new List<ILine>();

        public string ToHtml() {
            var sb = new StringBuilder();
            sb.AppendLine("<blockquote>");
            foreach (var text in Texts) {
                sb.AppendLine(text.ToHtml());
            }

            foreach (var text in Authors) {
                sb.AppendLine("<cite>" + text.ToHtml() + "</cite>");
            }

            sb.AppendLine("</blockquote>");
            return sb.ToString();
        }
    }
}
