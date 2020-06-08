# EbookConverter
Инструмент для конвертации fb2 и epub файлов в pdf

Для работы под Windows никаких телодвижений делать не надо. Все рабоатет из коробки

Для работы под Linux надо установить https://wkhtmltopdf.org/downloads.html#stable

ebookconverter -s "d:\epub" -d "d:\epub", где -s - папка и исходными файлами, -d - результирующая папка с pdf

Для полного списка опций можно вызвать 

ebookconverter --help

Для конвертации из fb2 используются заранее заданные шаблоны:

1. Шаблон обложки https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/cover.html
2. Шаблон основного контента https://github.com/OnlyFart/EbookConverter/blob/master/EbookConverter/Patterns/content.html

Если качество полученных pdf не устраивает или есть необходимость поменять стили, то это модно сделать через вышеуказанные шаблоны
