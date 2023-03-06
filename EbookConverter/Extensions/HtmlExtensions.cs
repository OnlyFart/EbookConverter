using System.Text;

namespace EbookConverter.Extensions {
    public static class HtmlExtensions {
        /// <summary>
        /// Обернуть строку <see cref="str"/> тегом <see cref="tag"/>
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static string ToHtmlTag(this string str, string tag, params string[] args) {
            var sb = new StringBuilder($"<{tag}");
            for (var i = 0; i < args.Length; i += 2) {
                var name = args[i];
                var value = args[i + 1];

                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(value)) {
                    sb.AppendThroughWhitespace($"{name}={value.CoverQuotes()}");
                }
            }

            sb.Append($">{str}</{tag}>");

            return sb.ToString();
        }

        /// <summary>
        /// Создание самозакрытого тега <see cref="tag"/>
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string SelfCloseHtmlTag(string tag, params string[] args) {
            var sb = new StringBuilder($"<{tag}");
            for (var i = 0; i < args.Length; i += 2) {
                sb.AppendThroughWhitespace($"{args[i]}={args[i + 1].CoverQuotes()}");
            }

            sb.Append("/>");

            return sb.ToString();
        }
    }
}
