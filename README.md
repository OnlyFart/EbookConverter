# EbookConverter
Инструмент для конвертации fb2 и epub файлов в pdf

## Для работы необходимо 
* [Net.core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)
* [Wkhtmltopdf](https://wkhtmltopdf.org/downloads.html#stable)

## Пример вызова сервиса
```
ebookconverter -s "<source_folder>" -d "<destination_folder>"
```
## Где 

```
-s - папка и исходными файлами
-d - результирующая папка с pdf
```

## Полный список опций 
```
ebookconverter --help
```

Для конвертации из fb2 используются заранее заданные шаблоны:

* [Шаблон обложки](https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/cover.html)
* [Шаблон основного контента](https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/content.html)

Если качество полученных pdf не устраивает или есть необходимость поменять стили, то это можно сделать через вышеуказанные шаблоны
