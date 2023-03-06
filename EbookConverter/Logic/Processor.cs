using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EbookConverter.Configs;
using EbookConverter.Converters;

namespace EbookConverter.Logic {
    public class Processor {
        private readonly List<ConverterBase> _converters;
        private readonly ProcessorConfig _processorConfig;

        public Processor(List<ConverterBase> converters, ProcessorConfig processorConfig) {
            if (processorConfig.MaxParallelThreads <= 0) {
                throw new ArgumentOutOfRangeException(nameof(processorConfig.MaxParallelThreads));
            }
            
            _converters = converters;
            _processorConfig = processorConfig;
        }
        
        /// <summary>
        /// Обработка одного файла
        /// </summary>
        /// <param name="source">Путь к файлу</param>
        /// <param name="destination">Путь к сконверченному файлу</param>
        /// <param name="wkArgs">Аргументы для запуска wkhtmltopdf</param>
        private async Task ProcessFile(string source, string destination, string wkArgs) {
            foreach (var converter in _converters.Where(converter => converter.IsSupport(source))) {
                try {
                    Console.WriteLine($"Start convert {source} to {destination}");
                    if (await converter.Convert(source, destination, wkArgs)) {
                        Console.WriteLine($"Success convert {source} to {destination}");
                    } else {
                        Console.WriteLine($"Fail convert {source} to {destination}");
                    }
                } catch (Exception e) {
                    Console.WriteLine($"Fail convert {source} to {destination}. Message {e.ToString()}");
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
        public async Task ProcessDirectory(string source, string destination, string pattern, string wkArgs) {
            if (!Directory.Exists(source)) {
                Console.WriteLine($"Not found source directory {source}");
                return;
            }
            
            if (!Directory.Exists(destination)) {
                Directory.CreateDirectory(destination);
            }
            
            await Parallel.ForEachAsync(Directory.GetFiles(source, pattern, SearchOption.AllDirectories), new ParallelOptions{ MaxDegreeOfParallelism = _processorConfig.MaxParallelThreads }, async (file, _) => {
                await ProcessFile(file, Path.Combine(destination, Path.GetFileNameWithoutExtension(file) + ".pdf"), wkArgs);
            });
        }
    }
}
