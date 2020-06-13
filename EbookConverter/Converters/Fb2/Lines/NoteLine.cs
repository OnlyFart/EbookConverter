using System.Collections.Generic;
using System.Linq;
using EbookConverter.Extensions;

namespace EbookConverter.Converters.Fb2.Lines {
    /// <summary>
    /// Пояснение
    /// </summary>
    public class NoteLine : ILine {
        public readonly List<ILine> Texts  = new List<ILine>();
        public readonly List<ILine> Titles = new List<ILine>();
        public string Id;
        
        public string ToHtml() {
            var pattern = "[{title}]"
                .ToHtmlTag("a", "href", "#{id}_backlink") // В качестве href используется backlink, что бы можно было вернуться в то место откуда был осуществлен переход
                .ToHtmlTag("strong") // Оборачивание ссылки в тег strong, что бы текст был жирным
                .AppendThroughWhitespace("{text}") // Шаблон для текста пояснения
                .ToHtmlTag("p", "id", "{id}"); // Все получившееся добро оборачиваем тегом p
            
            // Подмена айдишника, что бы работали ссылки
            pattern = Titles.Aggregate(pattern.Replace("{id}", Id), (current, title) => current.Replace("{title}", ((HeaderLine) title).Text.Trim()));
            
            // Подмена текста пояснения
            pattern = Texts.Aggregate(pattern, (current, title) => current.Replace("{text}", ((TextLine) title).Text));
            
            return pattern;
        }
    }
}
