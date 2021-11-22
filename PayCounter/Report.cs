using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using PayCounter.DB;

namespace PayCounter
{
    public static class Report
    {
        public static void MakeReport(string file, List<Payment> payments, string FIO)
        {
            IEnumerable<IGrouping<DB.Category, Payment>> grouped = payments.OrderBy(x => x.Date).GroupBy(x => x.Category);
            using (WordprocessingDocument doc = WordprocessingDocument.Create(file, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                // Шрифт
                RunFonts fonts = new RunFonts()
                {
                    Ascii = "Times New Roman",
                    EastAsia = "Times New Roman",
                    HighAnsi = "Times New Roman"
                };

                // Заголовок
                Paragraph TitleParagraph = body.AppendChild(new Paragraph(new ParagraphProperties()
                {
                    Justification = new Justification() { Val = JustificationValues.Center },
                }));
                Run TitleRun = new Run(new Text("Список платежей"));
                TitleRun.PrependChild(new RunProperties()
                {
                    FontSize = new FontSize() { Val = "48" },
                    RunFonts = (RunFonts)fonts.Clone(),
                    Bold = new Bold() { Val = DocumentFormat.OpenXml.OnOffValue.FromBoolean(true) }
                });
                TitleParagraph.AppendChild(TitleRun);

                // Свойства для категорий
                RunProperties GroupRunProperties = new RunProperties()
                {
                    FontSize = new FontSize() { Val = "28" },
                    RunFonts = (RunFonts)fonts.Clone(),
                    Bold = new Bold() { Val = DocumentFormat.OpenXml.OnOffValue.FromBoolean(true) }
                };

                // Свойства для платежей
                RunProperties PayRunProperties = new RunProperties()
                {
                    FontSize = new FontSize() { Val = "24" },
                    RunFonts = (RunFonts)fonts.Clone(),

                };

                // Свойства для итого
                ParagraphProperties totalProperties = new ParagraphProperties()
                {
                    Justification = new Justification() { Val = JustificationValues.Right },
                };
                RunProperties totalRunProperties = new RunProperties()
                {
                    FontSize = new FontSize() { Val = "32" },
                    RunFonts = (RunFonts)fonts.Clone()
                };

                // Табуляция
                ParagraphProperties pProperties = new ParagraphProperties(new Tabs(new TabStop()
                {
                    Val = TabStopValues.Right,
                    Position = 9072,
                    Leader = TabStopLeaderCharValues.Dot
                }));

                foreach (var group in grouped)
                {
                    Paragraph GroupParagraph = body.AppendChild(new Paragraph());

                    Run GroupRun = new Run(new Text(group.Key.Name));
                    GroupRun.PrependChild((RunProperties)GroupRunProperties.Clone());

                    GroupParagraph.AppendChild(GroupRun);

                    foreach (var payment in group)
                    {
                        Paragraph p = body.AppendChild(new Paragraph());
                        p.AppendChild((ParagraphProperties)pProperties.Clone());

                        Run DescRun = new Run(new Text(payment.Description));
                        DescRun.PrependChild((RunProperties)PayRunProperties.Clone());
                        p.AppendChild((Run)DescRun.Clone());

                        Run TabRun = new Run(new TabChar());
                        DescRun.PrependChild((RunProperties)PayRunProperties.Clone());
                        p.AppendChild((Run)TabRun.Clone());

                        Run CostRun = new Run(new Text(payment.Cost.ToString()));
                        CostRun.PrependChild((RunProperties)PayRunProperties.Clone());
                        p.AppendChild((Run)CostRun.Clone());

                    }
                }
                Paragraph total = body.AppendChild(new Paragraph());
                total.AppendChild((ParagraphProperties)totalProperties.Clone());
                Run totalRun = new Run(new Text("Итого: " + payments.Sum(x => x.Cost).ToString()));
                totalRun.PrependChild((RunProperties)totalRunProperties.Clone());
                total.AppendChild((Run)totalRun.Clone());


                //body.AppendChild(new Footer(new Paragraph(new ParagraphProperties(new ParagraphStyleId() { Val = "Footer" }, new Justification() { Val = JustificationValues.Center }), new Run(new SimpleField() { Instruction = "Page" }))));

                //doc.Save();
                doc.Close();
            }
        }
    }
}
