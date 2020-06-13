using System;
using System.Text;
using EbookConverter.Converters.Fb2.Lines;
using EbookConverter.Extensions;
using FB2Library.Elements;

namespace EbookConverter.Converters.Fb2 {
    public static class Fb2Extensions {
        /// <summary>
        /// Обертка для создания абзаца
        /// </summary>
        /// <param name="text">Текст абзаца</param>
        /// <returns></returns>
        public static TextLine ToTextLine(this string text) {
            return new TextLine {Text = text};
        }
        
        /// <summary>
        /// Обертка для создания заголовка
        /// </summary>
        /// <param name="text">Текст заголовка</param>
        /// <param name="id">Идентификатор заголовка</param>
        /// <param name="level">Уровень заголовка</param>
        /// <returns></returns>
        public static HeaderLine ToHeaderLine(this string text, string id, int level) {
            return new HeaderLine {Text = text, Id = id, HeaderLevel = level};
        }
        
        /// <summary>
        /// Обертка для создания картинки
        /// </summary>
        /// <param name="item">Картинка</param>
        /// <param name="id">Идентификатор картинки</param>
        /// <returns></returns>
        public static ImageLine ToImageLine(this BinaryItem item, string id) {
            return new ImageLine { Key = item.Id, Id = id};
        }
        
        /// <summary>
        /// Преобразования объекта типв <see cref="StyleType"/> в html
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string ToHtml(this StyleType item) {
            return item switch {
                SimpleText text => text.ToHtml(),
                InternalLinkItem link => link.ToHtml(),
                InlineImageItem image => image.ToHtml(),
                _ => item.ToString()
            };
        }
        
        /// <summary>
        /// Преобразование встроенного изображения в html
        /// </summary>
        /// <param name="image">Встроенное изображение</param>
        /// <returns></returns>
        private static string ToHtml(this InlineImageItem image) {
            var key = image.HRef.Replace("#", string.Empty);
            return HtmlExtensions.SelfCloseHtmlTag("img", "alt", key, "src", key, "type", "inline");
        }
        
        /// <summary>
        /// Преобразование встроенной ссылки в html
        /// </summary>
        /// <param name="link">Встроенная ссылка</param>
        /// <returns></returns>
        private static string ToHtml(this InternalLinkItem link) {
            // Для ссылок являющихся пояснением используется немного другая логика
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
        
        /// <summary>
        /// Преобразование "простого" текста в html
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToHtml(this SimpleText text) {
            // Если у объекта нет дочерних элементов, то сразу преобразуем его в html
            if (!text.HasChildren) {
                return ToHtml(text.Style, text.Text);
            }
            
            // При наличии дочерних объектов выполняется их рекурсивный обход
            // С последующим преобразованием в html
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
            
            // После рекурсивного обходы оборачиваем получившийся html
            // В тег, который соответствует стилю родительского элемента
            return ToHtml(text.Style, sb.ToString());
        }
        
        /// <summary>
        /// Обернуть строку <see cref="str"/> в тег, который соответсвует стилю <see cref="style"/>
        /// </summary>
        /// <param name="style">Стиль объекта</param>
        /// <param name="str">Строка текста</param>
        /// <returns></returns>
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
