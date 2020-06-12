namespace EbookConverter.Configs {
    /// <summary>
    /// Глобальный конфиг приложения
    /// </summary>
    public class GlobalConfig {
        /// <summary>
        /// Конфиг html шаблонов для генерации pdf
        /// </summary>
        public HtmlPatternsConfig HtmlPatternsConfig { get; set; }
        
        /// <summary>
        /// Конфиг для утилиты wkhtmltopdf
        /// </summary>
        public WkhtmltopdfConfig WkhtmltopdfConfig { get; set; }
    }
}
