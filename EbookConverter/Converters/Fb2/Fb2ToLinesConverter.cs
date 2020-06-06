using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EbookConverter.Converters.Fb2.Lines;
using FB2Library;
using FB2Library.Elements;
using FB2Library.Elements.Poem;

namespace EbookConverter.Converters.Fb2 {
    /// <summary>
    /// Конвертер вложенной структуры fb2 файла в плоский список
    /// </summary>
    public static class Fb2ToLinesConverter {
        /// <summary>
        /// Конвертация файла fb2 в плоский список
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static IEnumerable<ILine> Convert(FB2File file) {
            var lines = new List<ILine>();
            
            if (file.MainBody != null) {
                foreach (var bodyItem in file.Bodies) {
                    if (bodyItem.Name == "notes") {
                        AddTitle(bodyItem.Title, lines, string.Empty, 1);
                    }
                
                    foreach (var sectionItem in bodyItem.Epigraphs) {
                        PrepareTextItem(sectionItem, file, lines, bodyItem.Name, 1);
                    }

                    foreach (var sectionItem in bodyItem.Sections) {
                        PrepareTextItem(sectionItem, file, lines, bodyItem.Name, 1);
                    }
                }
            }

            return lines;
        }

        private static void PrepareTextItems(IEnumerable<IFb2TextItem> textItems, FB2File file, List<ILine> lines, string bodyName, int headerLevel) {
            foreach (var textItem in textItems) {
                if (textItem is IFb2TextItem) {
                    PrepareTextItem(textItem, file, lines, bodyName, headerLevel);
                } else {
                    lines.Add(textItem.ToString().ToTextLine());
                }
            }
        }

        private static void PrepareTextItem(IFb2TextItem textItem, FB2File file, List<ILine> lines, string bodyName, int headerLevel) {
            switch (textItem) {
                case CiteItem citeItem:
                    var cite = new Epigraph();
                    
                    PrepareTextItems(citeItem.CiteData, file, cite.Texts, bodyName, headerLevel);
                    PrepareTextItems(citeItem.TextAuthors, file, cite.Authors, bodyName, headerLevel);
                    
                    lines.Add(cite);
                    return;
                case PoemItem poemItem: {
                    AddTitle(poemItem.Title, lines, poemItem.ID, headerLevel);
                    PrepareTextItems(poemItem.Epigraphs, file, lines, bodyName, headerLevel + 1);
                    PrepareTextItems(poemItem.Content, file, lines, bodyName, headerLevel + 1);
                    return;
                }
                case SectionItem sectionItem: {
                    if (bodyName == "notes") {
                        var note = new NoteLine{Id = sectionItem.ID};
                        
                        AddTitle(sectionItem.Title, note.Titles, sectionItem.ID, headerLevel);
                        PrepareTextItems(sectionItem.Content, file, note.Texts, bodyName, headerLevel);
                    
                        lines.Add(note);
                    } else {
                        AddTitle(sectionItem.Title, lines, sectionItem.ID, headerLevel);
                        PrepareTextItems(sectionItem.Epigraphs, file, lines, bodyName, headerLevel + 1);
                        PrepareTextItems(sectionItem.Content, file, lines, bodyName, headerLevel + 1);
                    }

                    return;
                }
                case StanzaItem stanzaItem: {
                    AddTitle(stanzaItem.Title, lines, string.Empty, headerLevel);
                    PrepareTextItems(stanzaItem.Lines, file, lines, bodyName, headerLevel + 1);
                    return;
                }
                case SimpleText text: {
                    lines.Add(text.ToHtml().ToTextLine());
                    break;
                }
                case EmptyLineItem empty: {
                    lines.Add(empty.ToString().ToTextLine());
                    break;
                }
                case TitleItem title: {
                    AddTitle(title, lines, string.Empty, headerLevel + 1);
                    break;
                }
                case SubTitleItem _:
                case ParagraphItem _: {
                    var sb = new StringBuilder();
                    foreach (var paragraphData in ((ParagraphItem)textItem).ParagraphData) {
                        sb.Append(paragraphData.ToHtml());
                    }

                    lines.Add(sb.ToString().ToTextLine());

                    return;
                }
                case ImageItem imageItem: {
                    var key = imageItem.HRef.Replace("#", string.Empty);

                    if (file.Images.TryGetValue(key, out var image)) {
                        lines.Add(image.ToImageLine(key, imageItem.ID));
                    }

                    return;
                }
                case DateItem item:
                    lines.Add(item.DateValue.ToString().ToTextLine());
                    return;
                case EpigraphItem epigraphItem: {
                    var epigraph = new Epigraph();
                    
                    PrepareTextItems(epigraphItem.EpigraphData, file, epigraph.Texts, bodyName, headerLevel);
                    PrepareTextItems(epigraphItem.TextAuthors, file, epigraph.Authors, bodyName, headerLevel);
                    
                    lines.Add(epigraph);
                    return;
                }
                default:
                    throw new Exception(textItem.GetType().ToString());
            }
        }

        private static void AddTitle(TitleItem titleItem, List<ILine> lines, string id, int level) {
            if (titleItem != null) {
                lines.AddRange(titleItem.TitleData.Select(title => title.ToHeaderLine(id, level)));
            }
        }
    }
}