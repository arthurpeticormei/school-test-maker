using iTextSharp.text;
using iTextSharp.text.pdf;

namespace TestMaker.API.Utils
{
    public static class PdfTools
    {
        public static Font NewFont(string type, float size, BaseColor color)
        {
            Font font = FontFactory.GetFont(type, size, color);
            return font;
        }

        public static PdfContentByte NewPageBorder(Document doc, PdfWriter writer, int padding, BaseColor color)
        {
            PdfContentByte content = writer.DirectContent;
            Rectangle rectangle = new Rectangle(doc.PageSize);
            rectangle.Left += padding;
            rectangle.Right -= padding;
            rectangle.Top -= padding;
            rectangle.Bottom += padding;
            content.SetColorStroke(color);
            content.Rectangle(rectangle.Left, rectangle.Bottom, rectangle.Width, rectangle.Height);
            content.Stroke();
            return content;
        }

        public static PdfPTable NewTable(int numColums, float width = 100, int alignment = 0)
        {
            PdfPTable table = new PdfPTable(numColums)
            {
                WidthPercentage = width,
                HorizontalAlignment = alignment,
            };
            return table;
        }

        public static PdfPTable NewCell(PdfPTable table, string desc, Font font, int colSpan = 1, int alignment = 0, bool hasBorder = true, float padding = 5, float paddingBottom = 5)
        {
            Phrase content = new Phrase
            {
                new Chunk(desc, font),
            };
            PdfPCell cell = new PdfPCell(content)
            {
                Colspan = colSpan,
                Padding = padding,
                PaddingBottom = paddingBottom,
                HorizontalAlignment = alignment,
            };
            if (!hasBorder) cell.BorderWidth = 0;
            table.AddCell(cell);
            return table;
        }

        public static PdfPTable NewTitledCell(PdfPTable table, string title, string desc, Font fontTitle, Font fontDesc, int colSpan = 1, int alignment = 0, bool hasBorder = true, float padding = 5)
        {
            Phrase content = new Phrase
            {
                new Chunk(title, fontTitle),
                new Chunk("\n", fontTitle),
                new Chunk(desc, fontDesc),
            };
            PdfPCell cellInfo = new PdfPCell(content)
            {
                Colspan = colSpan,
                Padding = padding,
                HorizontalAlignment = alignment,
            };
            table.AddCell(cellInfo);
            if (!hasBorder) cellInfo.BorderWidth = 0;
            return table;
        }
    }
}
