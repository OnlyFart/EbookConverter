using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using EbookConverter.Converters;

namespace EbookConverter.Logic {
    public class Processor {
        private readonly List<ConverterBase> _converters;

        public Processor(List<ConverterBase> converters) {
            _converters = converters;
        }
        
        /// <summary>
        /// Обработка одного файла
        /// </summary>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        private void ProcessFile(string source, string destination, string wkArgs) {
            foreach (var converter in _converters) {
                if (converter.IsSupport(source)) {
                    try {
                        Console.WriteLine($"Start convert {source} to {destination}");
                        if (converter.Convert(source, destination, wkArgs)) {
                            Console.WriteLine($"Success convert {source} to {destination}");
                        } else {
                            Console.WriteLine($"Fail convert {source} to {destination}");
                        }
                    } catch (Exception e) {
                        Console.WriteLine($"Fail convert {source} to {destination}. Message {e.Message}");
                    }
                }
            }
        }
        
        /// <summary>
        /// Обработка директории
        /// </summary>
        /// <param name="source">Путь к директории</param>
        /// <param name="destination">Путь к реузльтирующей директории</param>
        /// <param name="pattern">Шаблон для поска файлов в <see cref="source"/></param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        public void ProcessDirectory(string source, string destination, string pattern, string wkArgs) {
            if (!Directory.Exists(source)) {
                Console.WriteLine($"Not found source directory {source}");
                return;
            }
            
            if (!Directory.Exists(destination)) {
                Directory.CreateDirectory(destination);
            }
            
            Parallel.ForEach(Directory.GetFiles(source, pattern, SearchOption.AllDirectories), new ParallelOptions{ MaxDegreeOfParallelism = 10 }, file => {
                ProcessFile(file, Path.Combine(destination, Path.GetFileNameWithoutExtension(file) + ".pdf"), wkArgs);
            });
        }
    }
}
