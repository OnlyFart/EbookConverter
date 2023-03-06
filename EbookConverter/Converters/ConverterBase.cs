using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EbookConverter.Configs;
using EbookConverter.Extensions;
using TempFolder;

namespace EbookConverter.Converters {
    /// <summary>
    /// Базовый класс конвертера
    /// </summary>
    public abstract class ConverterBase {
        private readonly string _extension;
        private readonly WkhtmltopdfConfig _config;

        protected ConverterBase(string extension, WkhtmltopdfConfig config) {
            _extension = extension;
            _config = config;
        }

        /// <summary>
        /// Проверка на поддержку файла данным конвертером
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && string.Equals(Path.GetExtension(path), _extension, StringComparison.InvariantCultureIgnoreCase);
        }
        
        /// <summary>
        /// Конвертация файла
        /// </summary>
        /// <param name="temp">Путь к временной папке для хранения промежуточных данных. После выхода из функции папка будет удалена</param>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected abstract Task<bool> ConvertInternal(string temp, string source, string destination, string wkArgs);
        
        /// <summary>
        /// Конвертация файла
        /// </summary>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        public async Task<bool> Convert(string source, string destination, string wkArgs) {
            using var temp = TempFolderFactory.Create();
            return await ConvertInternal(temp.Path, source, destination, wkArgs);
        }

        /// <summary>
        /// Генерация pdf файла
        /// </summary>
        /// <param name="cover">Путь к файлу с обложкой</param>
        /// <param name="content">Путь к файлу с контентом</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected Task<bool> GeneratePdf(string cover, string content, string destination, string wkArgs) {
            return GeneratePdf(cover, new List<string> {content}, destination, wkArgs);
        }
        
        /// <summary>
        /// Генерация pdf файла
        /// </summary>
        /// <param name="cover">Путь к файлу с обложкой</param>
        /// <param name="contents">Список файлов с контентом</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected async Task<bool> GeneratePdf(string cover, List<string> contents, string destination, string wkArgs) {
            if (contents.Count == 0 && string.IsNullOrWhiteSpace(cover)) {
                throw new Exception("No pages for convert");
            }

            var arguments = string.IsNullOrWhiteSpace(wkArgs) ? _config.DefaultArgs : wkArgs;

            if (!string.IsNullOrEmpty(cover)) {
                arguments = arguments
                    .AppendThroughWhitespace("cover")
                    .AppendThroughWhitespace(cover.CoverQuotes());
            }
            
            arguments = arguments
                .AppendThroughWhitespace(contents.Select(path => path.CoverQuotes()))
                .AppendThroughWhitespace(destination.CoverQuotes());

            var info = new ProcessStartInfo {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                FileName = _config.Path,
                Arguments = arguments
            };

            using var process = Process.Start(info);
            if (process == null) {
                throw new Exception("Fail to start wkhtmltopdf process");
            }
            
            await process.WaitForExitAsync();
            return process.ExitCode == 0;
        }
    }
}
