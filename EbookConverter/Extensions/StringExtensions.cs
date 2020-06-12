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
    }
}
