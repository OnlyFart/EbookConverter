using System.Collections.Generic;
using System.Text;
using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines; 

public class Table : ILine {
    public List<TableRow> Rows = new();
    public string ToHtml() {
        var sb = new StringBuilder();

        foreach (var row in Rows) {
            sb.Append(row.ToHtml());
        }

        return sb.ToString().ToHtmlTag("table");
    }
}