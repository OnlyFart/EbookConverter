using System;
using System.Text;
using EbookConverter.Converters.Fb2.Lines;
using EbookConverter.Extensions;
using FB2Library.Elements;

namespace EbookConverter.Converters.Fb2 {
    public static class Fb2Extensions {
        public static TextLine ToTextLine(this string text) {
            return new TextLine {Text = text};
        }
        
        public static HeaderLine ToHeaderLine(this IFb2TextItem item, string id, int level) {
            return new HeaderLine {Text = item.ToString(), Id = id, HeaderLevel = level};
        }

        public static ImageLine ToImageLine(this BinaryItem item, string id) {
            return new ImageLine { Key = item.Id, Id = id};
        }
        
        public static string ToHtml(this StyleType item) {
            return item switch {
                SimpleText text => text.ToHtml(),
                InternalLinkItem link => link.ToHtml(),
                InlineImageItem image => image.ToHtml(),
                _ => item.ToString()
            };
        }

        private static string ToHtml(this InlineImageItem image) {
            var key = image.HRef.Replace("#", string.Empty);
            return HtmlExtensions.SelfCloseHtmlTag("img", "alt", key, "src", key, "type", "inline");
        }

        private static string ToHtml(this InternalLinkItem link) {
            if (link.Type == "note") {
                return link.LinkText
                    .ToHtml()
                    .ToHtmlTag("a", "href", link.HRef, "type", link.Type, "id", link.HRef.Trim('#') + "_backlink")
                    .ToHtmlTag("sup");
            }

            return link.LinkText
                .ToHtml()
                .ToHtmlTag("a", "href", link.HRef, "type", link.Type);
        }
        
        public static string ToHtml(this SimpleText text) {
            if (!text.HasChildren) {
                return ToHtml(text.Style, text.Text);
            }
            
            var sb = new StringBuilder();
            foreach (var child in text.Children) {
                switch (child) {
                    case SimpleText simple:
                        sb.Append(simple.ToHtml());
                        break;
                    case InternalLinkItem link:
                        sb.Append(link.ToHtml());
                        break;
                    case InlineImageItem image:
                        sb.Append(image.ToHtml());
                        break;
                    default:
                        sb.Append(ToHtml(text.Style, child.ToString()));
                        break;
                }
            }

            return ToHtml(text.Style, sb.ToString());
        }

        private static string ToHtml(TextStyles style, string str) {
            return style switch {
                TextStyles.Normal => str,
                TextStyles.Strong => str.ToHtmlTag("strong"),
                TextStyles.Emphasis => str.ToHtmlTag("i"),
                TextStyles.Code => str.ToHtmlTag("pre"),
                TextStyles.Sub => str.ToHtmlTag("sub"),
                TextStyles.Sup => str.ToHtmlTag("sup"),
                TextStyles.Strikethrough => str.ToHtmlTag("strike"),
                _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
            };
        }
    }
}
