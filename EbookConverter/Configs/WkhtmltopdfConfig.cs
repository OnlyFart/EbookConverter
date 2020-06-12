namespace EbookConverter.Configs {
    public class WkhtmltopdfConfig {
        /// <summary>
        /// Аргументы для запуска утилиты wkhtmltopdf по умолчанию
        /// </summary>
        public string DefaultArgs { get; set; }
        
        /// <summary>
        /// Путь до утилиты
        /// </summary>
        public string Path { get; set; }
    }
}
