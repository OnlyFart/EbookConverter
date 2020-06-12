using System.Collections.Generic;
using System.Linq;

namespace EbookConverter.Extensions {
    public static class StringExtensions {
        /// <summary>
        /// Обернуть строку кавычками
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CoverQuotes(this string str) {
            return "\"" + str + "\"";
        }
        
        /// <summary>
        /// Добавить к <see cref="str"/> строку <see cref="append"/> через пробел
        /// </summary>
        /// <param name="str"></param>
        /// <param name="append"></param>
        /// <returns></returns>
        public static string AppendThroughWhitespace(this string str, string append) {
            return str + " " + append;
        }
        
        /// <summary>
        /// Добавить к <see cref="str"/> строки <see cref="appends"/> через пробел
        /// </summary>
        /// <param name="str"></param>
        /// <param name="appends"></param>
        /// <returns></returns>
        public static string AppendThroughWhitespace(this string str, IEnumerable<string> appends) {
            return appends.Aggregate(str, (current, append) => current.AppendThroughWhitespace(append));
        }
    }
}
