using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FB2Library;
using FB2Library.Elements;
using FB2Library.Elements.Poem;

namespace EbookConverter.Converters.Fb2 {
    public class Fb2ToLinesConverter {
        public static List<ILine> Convert(FB2File file) {
            var result = new List<ILine>();
            
            if (file.MainBody != null) {
                PrepareBodies(file, result);
            }

            return result;
        }

        private static void PrepareBodies(FB2File file, List<ILine> lines) {
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

        private static void PrepareTextItems(IEnumerable<IFb2TextItem> textItems, FB2File file, List<ILine> lines, string bodyName, int headerLevel) {
            foreach (var textItem in textItems) {
                if (textItem is IFb2TextItem) {
                    PrepareTextItem(textItem, file, lines, bodyName, headerLevel);
                } else {
                    AddTextLine(textItem.ToString(), lines);
                }
            }
        }

        private static void PrepareTextItem(IFb2TextItem textItem, FB2File file, List<ILine> lines, string bodyName, int headerLevel) {
            switch (textItem) {
                case CiteItem item:
                    var cite = new Epigraph();
                    
                    PrepareTextItems(item.CiteData, file, cite.Texts, bodyName, headerLevel);
                    PrepareTextItems(item.TextAuthors, file, cite.Authors, bodyName, headerLevel);
                    
                    lines.Add(cite);
                    return;
                case PoemItem poemItem: {
                    AddTitle(poemItem.Title, lines, poemItem.ID, headerLevel);
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
                    AddTextLine(text.ToHtml(), lines);
                    break;
                }
                case EmptyLineItem empty: {
                    AddTextLine(empty.ToString(), lines);
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

                    AddTextLine(sb.ToString(), lines);

                    return;
                }
                case ImageItem imageItem: {
                    var key = imageItem.HRef.Replace("#", string.Empty);

                    if (file.Images.ContainsKey(key)) {
                        var data = file.Images[key].BinaryData;
                        lines.Add(new ImageLine {Data = data, Key = key, Id = imageItem.ID});
                    }

                    return;
                }
                case DateItem item:
                    AddTextLine(item.DateValue.ToString(), lines);
                    return;
                case EpigraphItem epigraphItem: {
                    var epigraf = new Epigraph();
                    
                    PrepareTextItems(epigraphItem.EpigraphData, file, epigraf.Texts, bodyName, headerLevel);
                    PrepareTextItems(epigraphItem.TextAuthors, file, epigraf.Authors, bodyName, headerLevel);
                    
                    lines.Add(epigraf);
                    return;
                }
                default:
                    throw new Exception(textItem.GetType().ToString());
            }
        }

        private static void AddTitle(TitleItem titleItem, List<ILine> lines, string id, int level) {
            if (titleItem != null) {
                foreach (var title in titleItem.TitleData) {
                    lines.Add(new HeaderLine {Text = title.ToString(), Id = id, HeaderLevel = level});
                }
            }
        }

        private static void AddTextLine(string text, List<ILine> lines) {
            lines.Add(new TextLine {Text = text});
        }
    }
}