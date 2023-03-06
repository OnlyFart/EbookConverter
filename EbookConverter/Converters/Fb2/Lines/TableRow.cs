using System.Collections.Generic;
using System.Text;
using EbookConverter.Extensions;
using FB2Library.Elements.Table;

namespace EbookConverter.Converters.Fb2.Lines; 

public class TableRow : ILine {
    public List<ICellElement> Cells = new();
    
    public string ToHtml() {
        var sb = new StringBuilder();

        foreach (var cell in Cells) {
            sb.Append(cell.ToHtml());
        }

        return sb.ToString().ToHtmlTag("tr");
    }
}