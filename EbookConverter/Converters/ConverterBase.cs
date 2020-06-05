using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace EbookConverter.Converters {
    public abstract class ConverterBase {
        /// <summary>
        /// Проверка на поддержку файла данным экстрактором
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public abstract bool IsSupport(string path);

        protected abstract bool ConvertInternal(string tempPath, string sourcePath, string destinationPath, string wkArgs);

        public bool Convert(string sourcePath, string destinationPath, string wkArgs) {
            var tempPath = GetTemporaryDirectory();
            try {
                return ConvertInternal(tempPath, sourcePath, destinationPath, wkArgs);
            } finally {
                Directory.Delete(tempPath, true);
            } 
        }
        
        protected static string GetTemporaryDirectory() {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        protected static bool GeneratePdf(string cover, string content, string outPath, string wkArgs) {
            return GeneratePdf(cover, new List<string> {content}, outPath, wkArgs);
        }
        
        protected static bool GeneratePdf(string cover, List<string> contents, string outPath, string wkArgs) {
            if (contents.Count == 0) {
                throw new Exception("No pages for convert");
            }

            var arguments = $"/c wkhtmltopdf {wkArgs} ";

            if (!string.IsNullOrEmpty(cover)) {
                arguments += $"cover {cover} ";
            }
            
            arguments += string.Join(" ", contents);
            arguments += " \"" + outPath + "\"";
            
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
