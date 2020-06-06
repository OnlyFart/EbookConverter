using System;
using FB2Library.Elements;

namespace EbookConverter.Converters.Fb2 {
    public static class Fb2Extensions {
        public static string ToHtml(this StyleType item) {
            switch (item) {
                case SimpleText text:
                    return text.ToHtml();
                case InternalLinkItem link:
                    return link.ToHtml();

                case InlineImageItem image:
                    return image.ToHtml();

                default:
                    return item.ToString();
            }
        }

        public static string ToHtml(this InlineImageItem image) {
            var key = image.HRef.Replace("#", string.Empty);
            return $"<img alt=\"{key}\" src=\"{key}\" />";
        }
        
        public static string ToHtml(this InternalLinkItem link) {
            if (link.Type == "note") {
                return $"<sup><a href=\"{link.HRef}\" type=\"{link.Type}\" id=\"{link.HRef.Trim('#')}_backlink\">{link.LinkText.ToHtml()}</a></sup>";
            } else {
                return $"<a href=\"{link.HRef}\" type=\"{link.Type}\">{link.LinkText.ToHtml()}</a>";
            }
        }
        
        public static string ToHtml(this SimpleText text) {
            if (!text.HasChildren) {
                return string.Format(GetHtmlTextPattern(text.Style), text.Text);
            }

            foreach (var child in text.Children) {
                switch (child) {
                    case SimpleText simple:
                        return ToHtml(simple);
                    case InternalLinkItem link:
                        return ToHtml(link.LinkText);
                    default:
                        return child.ToString();
                }
            }

            return string.Empty;
        }

        private static string GetHtmlTextPattern(TextStyles style) {
            switch (style) {
                case TextStyles.Normal:
                    return "{0}";
                case TextStyles.Strong:
                    return "<strong>{0}</strong>";
                case TextStyles.Emphasis:
                    return "<i>{0}</i>";
                case TextStyles.Code:
                    return "<pre>{0}</pre>";
                case TextStyles.Sub:
                    return "<sub>{0}</sub>";
                case TextStyles.Sup:
                    return "<sup>{0}</sup>";
                case TextStyles.Strikethrough:
                    return "<strike>{0}</strike>";
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }
    }
}
