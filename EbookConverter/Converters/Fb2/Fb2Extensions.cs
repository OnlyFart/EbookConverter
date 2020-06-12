using System;
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
            return HtmlExtensions.SelfCloseHtmlTag("img", "alt", key, "src", key);
        }

        private static string ToHtml(this InternalLinkItem link) {
            if (link.Type == "note") {
                return link.LinkText
                    .ToHtml()
                    .ToHtmlTag("a", "href", link.HRef, "type", link.Type, "id", link.HRef.Trim('#') + "_backlink")
                    .ToHtmlTag("sup");
            }

            return ToHtml((StyleType) link.LinkText).ToHtmlTag("a", "href", link.HRef, "type", link.Type);
        }
        
        public static string ToHtml(this SimpleText text) {
            if (!text.HasChildren) {
                return string.Format(GetHtmlTextPattern(text.Style), text.Text);
            }

            foreach (var child in text.Children) {
                return child switch {
                    SimpleText simple => ToHtml(simple),
                    InternalLinkItem link => ToHtml(link.LinkText),
                    _ => child.ToString()
                };
            }

            return string.Empty;
        }

        private static string GetHtmlTextPattern(TextStyles style) {
            const string PATTERN = "{0}";

            return style switch {
                TextStyles.Normal => PATTERN,
                TextStyles.Strong => PATTERN.ToHtmlTag("strong"),
                TextStyles.Emphasis => PATTERN.ToHtmlTag("i"),
                TextStyles.Code => PATTERN.ToHtmlTag("pre"),
                TextStyles.Sub => PATTERN.ToHtmlTag("sub"),
                TextStyles.Sup => PATTERN.ToHtmlTag("sup"),
                TextStyles.Strikethrough => PATTERN.ToHtmlTag("strike"),
                _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
            };
        }
    }
}
