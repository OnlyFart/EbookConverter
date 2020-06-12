# EbookConverter
Инструмент для конвертации fb2 и epub файлов в pdf

Для работы необходим net.core 3.1 https://dotnet.microsoft.com/download/dotnet-core/3.1

А так же утилита https://wkhtmltopdf.org/downloads.html#stable

Пример вызова сервиса
```
ebookconverter -s "d:\epub" -d "d:\epub"
```
Где -s - папка и исходными файлами, -d - результирующая папка с pdf

Для полного списка опций можно вызвать 

```
ebookconverter --help
```

Для конвертации из fb2 используются заранее заданные шаблоны:

1. Шаблон обложки https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/cover.html
2. Шаблон основного контента https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/content.html

Если качество полученных pdf не устраивает или есть необходимость поменять стили, то это можно сделать через вышеуказанные шаблоны
