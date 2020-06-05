using System;
using System.Collections.Generic;
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
                AddTitle(bodyItem.Title, lines, string.Empty, 1);

                foreach (var sectionItem in bodyItem.Sections) {
                    PrepareTextItem(sectionItem, file, lines, 1);
                }
            }
        }

        private static void PrepareTextItems(IEnumerable<IFb2TextItem> textItems, FB2File file, List<ILine> lines, int headerLevel) {
            foreach (var textItem in textItems) {
                if (textItem is IFb2TextItem) {
                    PrepareTextItem(textItem, file, lines, headerLevel);
                } else {
                    AddTextLine(textItem.ToString(), lines);
                }
            }
        }

        private static void PrepareTextItem(IFb2TextItem textItem, FB2File file, List<ILine> lines, int headerLevel) {
            switch (textItem) {
                case CiteItem item:
                    PrepareTextItems(item.CiteData, file, lines, headerLevel);
                    return;
                case PoemItem poemItem: {
                    var item = poemItem;
                    AddTitle(item.Title, lines, item.ID, headerLevel);
                    PrepareTextItems(item.Content, file, lines, headerLevel + 1);
                    return;
                }
                case SectionItem sectionItem: {
                    var item = sectionItem;
                    AddTitle(item.Title, lines, item.ID, headerLevel);
                    PrepareTextItems(item.Content, file, lines, headerLevel + 1);
                    return;
                }
                case StanzaItem stanzaItem: {
                    var item = stanzaItem;
                    AddTitle(item.Title, lines, string.Empty, headerLevel);
                    PrepareTextItems(item.Lines, file, lines, headerLevel + 1);
                    return;
                }
                case ParagraphItem _:
                case EmptyLineItem _:
                case TitleItem _:
                case SimpleText _: {
                    if (textItem is SubTitleItem) {
                        lines.Add(new SubTitle {Text = textItem.ToString()});
                    } else if (textItem is ParagraphItem) {
                        var sb = new StringBuilder();

                        foreach (var paragraphData in ((ParagraphItem) textItem).ParagraphData) {
                            if (paragraphData is SimpleText) {
                                sb.Append(paragraphData);
                            } else if (paragraphData is InternalLinkItem) {
                                var link = (InternalLinkItem) paragraphData;
                                sb.Append($"<a href=\"{link.HRef}\" type=\"{link.Type}\">{link.LinkText}</a>");
                            } else {
                                sb.Append(paragraphData);
                            }
                        }

                        AddTextLine(sb.ToString(), lines);
                    } else {
                        AddTextLine(textItem.ToString(), lines);
                    }

                    return;
                }
                case ImageItem imageItem: {
                    var item = imageItem;
                    var key = item.HRef.Replace("#", string.Empty);

                    if (file.Images.ContainsKey(key)) {
                        var data = file.Images[key].BinaryData;
                        lines.Add(new ImageLine {Data = data, Key = key});
                    }

                    return;
                }
                case DateItem item:
                    AddTextLine(item.DateValue.ToString(), lines);
                    return;
                case EpigraphItem epigraphItem: {
                    var item = epigraphItem;
                    PrepareTextItems(item.EpigraphData, file, lines, headerLevel);
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