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

        private void ProcessFile(string path, string destination, string wkArgs) {
            foreach (var converter in _converters) {
                if (converter.IsSupport(path)) {
                    try {
                        Console.WriteLine($"Start convert {path} to {destination}");
                        if (converter.Convert(path, destination, wkArgs)) {
                            Console.WriteLine($"Success convert {path} to {destination}");
                        } else {
                            Console.WriteLine($"Fail convert {path} to {destination}");
                        }
                    } catch (Exception e) {
                        Console.WriteLine($"Fail convert {path} to {destination}. Message {e.Message}");
                    }
                }
            }
        }

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
