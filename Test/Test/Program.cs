using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("t.pdf", FileMode.OpenOrCreate));

            //open
            document.Open();

            //hello world
            document.Add(new Paragraph("HELLO WORLD!"));

            //draw
            PdfContentByte content = writer.DirectContent;
            content.SetColorStroke(BaseColor.BLUE);
            content.MoveTo(72.0f, PageSize.A4.Height - 10);
            content.LineTo(300.0f, PageSize.A4.Height - 10);
            content.Stroke();

            content.SetColorFill(BaseColor.PINK);
            content.Ellipse(PageSize.A4.Width / 7, PageSize.A4.Height - 20, PageSize.A4.Width / 2, PageSize.A4.Height * 3 / 4);
            content.Fill();

            content.SetColorStroke(BaseColor.RED);
            content.Rectangle(PageSize.A4.Width / 7, PageSize.A4.Height * 3 / 4, PageSize.A4.Width / 2, PageSize.A4.Height / 5);
            content.Stroke();

            //close
            document.Close();
        }
    }
}
