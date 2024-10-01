using iTextSharp.text.pdf;
using iTextSharp.text;
using TestMaker.API.Models;
using TestMaker.API.Utils;

namespace TestMaker.API.Services
{
    public class TestMakerService
    {
        #region Test builder
        public string NewTest(Test test)
        {
            try
            {
                MemoryStream mem = new MemoryStream();
                using (mem)
                {
                    using (Document doc = new Document(PageSize.A4))
                    {
                        PdfWriter writer = PdfWriter.GetInstance(doc, mem);
                        doc.Open();
                        PdfContentByte content = PdfTools.NewPageBorder(doc, writer, 25, BaseColor.BLACK);
                        doc.Add(BuildHeader(test, PdfTools.NewFont("Helvetica", 11f, BaseColor.BLACK), PdfTools.NewFont("Helvetica-Bold", 10f, BaseColor.BLACK)));
                        doc.Add(BuildQuestions(test, PdfTools.NewFont("Helvetica-Bold", 11f, BaseColor.BLACK), PdfTools.NewFont("Helvetica", 11f, BaseColor.BLACK)));
                        doc.Close();
                    }
                }
                return Convert.ToBase64String(BuildPagination(mem));
            }
            catch (Exception)
            {
                throw;
            }
        }

        internal static byte[] BuildPagination(MemoryStream mem)
        {
            PdfReader reader = new PdfReader(mem.ToArray());
            Document doc = new Document(reader.GetPageSizeWithRotation(1));
            MemoryStream newMem = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(doc, newMem);
            doc.Open();
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                doc.NewPage();
                PdfTools.NewPageBorder(doc, writer, 25, BaseColor.BLACK);
                PdfImportedPage importedPage = writer.GetImportedPage(reader, i);
                PdfContentByte cb = writer.DirectContent;
                cb.AddTemplate(importedPage, 0, 0);
            }
            doc.Close();
            return newMem.ToArray();
        }
        #endregion

        #region Header builder
        private static PdfPTable BuildHeader(Test test, Font fontTitle, Font fontDesc)
        {
            TestHeader header = test.Header;
            PdfPTable table = PdfTools.NewTable(6);
            PdfPCell logoCell = new PdfPCell()
            {
                Padding = 5,
                HorizontalAlignment = 1,
            };
            logoCell.AddElement(Image.GetInstance(Convert.FromBase64String(header.LogoBase64)));
            table.AddCell(logoCell);
            table = BuildHeaderData(table, header, fontTitle, fontDesc, 5);
            return table;
        }

        private static PdfPTable BuildHeaderData(PdfPTable table, TestHeader header, Font fontTitle, Font fontDesc, int colSpan)
        {
            PdfPCell cell = new PdfPCell
            {
                Colspan = colSpan,
                Padding = 0,
            };
            PdfPTable innerTable = PdfTools.NewTable(5);
            innerTable = BuildHeaderCell(innerTable, "PROVA", header.TestType, fontTitle, fontDesc, 0, 0);
            innerTable = BuildHeaderCell(innerTable, "DISCIPLINA", header.Subject, fontTitle, fontDesc);
            innerTable = BuildHeaderCell(innerTable, "SÉRIE/TURMA", header.Class, fontTitle, fontDesc);
            innerTable = BuildHeaderCell(innerTable, "TURNO", header.Shift, fontTitle, fontDesc);
            innerTable = BuildHeaderCell(innerTable, "UNIDADE", header.Unit, fontTitle, fontDesc);
            innerTable = PdfTools.NewCell(innerTable, "ALUNO(A):", fontTitle, 5);
            innerTable = BuildHeaderCell(innerTable, "PROFESSOR(A):", header.Teacher, fontTitle, fontDesc, 0, 0, 2);
            innerTable = PdfTools.NewCell(innerTable, "DATA:_____/_____/2024", fontTitle, 2, 2, false);
            innerTable = PdfTools.NewCell(innerTable, "NOTA:", fontTitle);
            cell.AddElement(innerTable);
            table.AddCell(cell);
            return table;
        }

        private static PdfPTable BuildHeaderCell(PdfPTable table, string title, string desc, Font fontTitle, Font fontDesc, int titleAlign = 1, int descAlign = 1, int colSpan = 1)
        {
            PdfPCell cell = new PdfPCell
            {
                Colspan = colSpan,
                Border = 0,
                Padding = 0,
            };
            PdfPTable innerTable = PdfTools.NewTable(1);
            innerTable = PdfTools.NewCell(innerTable, title, fontTitle, 1, titleAlign, false, 5, 0);
            innerTable = PdfTools.NewCell(innerTable, desc, fontDesc, 1, descAlign, false);
            cell.AddElement(innerTable);
            table.AddCell(cell);
            return table;
        }
        #endregion

        #region Questions builder
        private static PdfPTable BuildQuestions(Test test, Font fontTitle, Font fontDesc)
        {
            PdfPTable table = PdfTools.NewTable(1);
            table.SpacingBefore = 5f;
            if (test.Questions != null)
            {
                int index = 1;
                test.Questions.ForEach(q => {
                    PdfPCell cell = new PdfPCell
                    {
                        Border = 0,
                        Padding = 0,
                    };
                    cell.AddElement(BuildQuestion(q, fontTitle, fontDesc, index++));
                    table.AddCell(cell);
                });
            }
            return table;
        }

        private static PdfPTable BuildQuestion(Question question, Font fontTitle, Font fontDesc, int index)
        {

            PdfPTable table = PdfTools.NewTable(7);
            table = PdfTools.NewCell(table, string.Format("QUESTÃO {0} - {1}", index, question.Statement), fontTitle, 6, 3, false);
            table = BuildQuestionScore(table, question);
            if (question.ImagesBase64 != null)
            {
                PdfPCell imagesCell = new PdfPCell
                {
                    Colspan = 7,
                    Border = 0,
                    Padding = 0,
                };
                PdfPTable imagesTable = PdfTools.NewTable(2);
                question.ImagesBase64.ForEach(i => {
                    PdfPCell imageCell = new PdfPCell()
                    {
                        Padding = 5,
                        Border = 0,
                    };
                    imageCell.AddElement(Image.GetInstance(Convert.FromBase64String(i)));
                    imagesTable.AddCell(imageCell);
                });
                if (question.ImagesBase64.Count % 2 != 0) imagesTable = PdfTools.NewCell(imagesTable, string.Empty, fontDesc, 1, 0, false);
                imagesCell.AddElement(imagesTable);
                table.AddCell(imagesCell);
            }
            if (question.Alternatives != null)
            {
                char character = 'a';
                question.Alternatives.ForEach(a => {
                    table = PdfTools.NewCell(table, string.Format("{0}) {1}", character, a), fontDesc, 7, 0, false);
                    character = (char)(character + 1);
                });
            }
            if (question.SpacingAfter > 0) table = PdfTools.NewCell(table, string.Concat(Enumerable.Repeat("\n", question.SpacingAfter)), fontDesc, 7, 0, false);
            return table;
        }

        private static PdfPTable BuildQuestionScore(PdfPTable tabela, Question question)
        {
            PdfPCell cell = new PdfPCell
            {
                Colspan = 1,
                Border = 0,
                PaddingTop = 5,
            };
            PdfPTable innerTable = PdfTools.NewTable(2);
            innerTable = PdfTools.NewCell(innerTable, "PONTUAÇÃO", PdfTools.NewFont("Helvetica-Bold", 9f, BaseColor.BLACK), 2, 1, true, 1);
            innerTable = PdfTools.NewCell(innerTable, question.Score.ToString("0.0"), PdfTools.NewFont("Helvetica-Bold", 9f, BaseColor.BLACK), 1, 1, true, 1);
            innerTable = PdfTools.NewCell(innerTable, string.Empty, PdfTools.NewFont("Helvetica-Bold", 9f, BaseColor.BLACK), 1);
            cell.AddElement(innerTable);
            tabela.AddCell(cell);
            return tabela;
        }
        #endregion
    }
}
