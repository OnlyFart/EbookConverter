using System;
using EbookConverter.Converters.Fb2.Lines;
using FB2Library.Elements;

namespace EbookConverter.Converters.Fb2 {
    public static class Fb2Extensions {
        public static TextLine ToTextLine(this string text) {
            return new TextLine {Text = text};
        }
        
        public static HeaderLine ToHeaderLine(this IFb2TextItem item, string id, int level) {
            return new HeaderLine {Text = item.ToString(), Id = id, HeaderLevel = level};
        }

        public static ImageLine ToImageLine(this BinaryItem item, string key, string id) {
            return new ImageLine {Data = item.BinaryData, Key = key, Id = id};
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
            return $"<img alt=\"{key}\" src=\"{key}\" />";
        }

        private static string ToHtml(this InternalLinkItem link) {
            if (link.Type == "note") {
                return $"<sup><a href=\"{link.HRef}\" type=\"{link.Type}\" id=\"{link.HRef.Trim('#')}_backlink\">{link.LinkText.ToHtml()}</a></sup>";
            }

            return $"<a href=\"{link.HRef}\" type=\"{link.Type}\">{ToHtml((StyleType) link.LinkText)}</a>";
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
            return style switch {
                TextStyles.Normal => "{0}",
                TextStyles.Strong => "<strong>{0}</strong>",
                TextStyles.Emphasis => "<i>{0}</i>",
                TextStyles.Code => "<pre>{0}</pre>",
                TextStyles.Sub => "<sub>{0}</sub>",
                TextStyles.Sup => "<sup>{0}</sup>",
                TextStyles.Strikethrough => "<strike>{0}</strike>",
                _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
            };
        }
    }
}
