using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace EbookConverter.Converters {
    /// <summary>
    /// Базовый класс конвертера
    /// </summary>
    public abstract class ConverterBase {
        private readonly string _extension;

        protected ConverterBase(string extension) {
            _extension = extension;
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
        protected abstract bool ConvertInternal(string temp, string source, string destination, string wkArgs);
        
        /// <summary>
        /// Конвертация файла
        /// </summary>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        public bool Convert(string source, string destination, string wkArgs) {
            var tempPath = GetTemporaryDirectory();
            try {
                return ConvertInternal(tempPath, source, destination, wkArgs);
            } finally {
                Directory.Delete(tempPath, true);
            } 
        }
        
        /// <summary>
        /// Создание временной директории для хранения промежуточных данных
        /// </summary>
        /// <returns></returns>
        private static string GetTemporaryDirectory() {
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;
        }
        
        /// <summary>
        /// Генерация pdf файла
        /// </summary>
        /// <param name="cover">Путь к файлу с обложкой</param>
        /// <param name="content">Путь к файлу с контентом</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        /// <returns></returns>
        protected static bool GeneratePdf(string cover, string content, string destination, string wkArgs) {
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
        protected static bool GeneratePdf(string cover, List<string> contents, string destination, string wkArgs) {
            if (contents.Count == 0) {
                throw new Exception("No pages for convert");
            }

            var arguments = $"/c wkhtmltopdf {wkArgs} ";

            if (!string.IsNullOrEmpty(cover)) {
                arguments += $"cover \"{cover}\" ";
            }
            
            arguments += string.Join(" ", contents.Select(path => $"\"{path}\""));
            arguments += " \"" + destination + "\"";
            
            var info = new ProcessStartInfo();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                info = new ProcessStartInfo {
                    WorkingDirectory = "wkhtmltopdf",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = "cmd.exe",
                    Arguments = arguments
                };
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                info = new ProcessStartInfo {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = "/bin/bash",
                    Arguments = arguments
                };
            }

            using (var process = Process.Start(info)) {
                if (process == null) {
                    throw new Exception("Fail to start wkhtmltopdf process");
                }

                process.WaitForExit();
                if (process.ExitCode > 0) {
                    return false;
                }

            }

            return true;
        }
    }
}
