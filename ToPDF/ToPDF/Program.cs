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
        const float lineSpace = 6;
        const float headSpace = 4;
        const float scoreRadius = lineSpace / 2;
        static void Main(string[] args)
        {
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("C:\\Users\\t-yaxie\\Desktop\\Fun\\MrChorder\\t.pdf", FileMode.OpenOrCreate));

            //Console.Write("%f %f", PageSize.A4.Height, PageSize.A4.Width);
            //open
            document.Open();

            //hello world
            document.Add(new Paragraph("HELLO WORLD!"));
            //842 595
            string parameter = PageSize.A4.Height + " " + PageSize.A4.Width;
            document.Add(new Paragraph(parameter));

            //draw
            PdfContentByte content = writer.DirectContent;

            /*
            DrawFiveLines(content, 100, 500, 100);
            DrawOneScore(content, 120, 100, 0);
            DrawOneScore(content, 140, 100, -1);
            DrawOneScore(content, 160, 100, 14);
            DrawOneScore(content, 180, 100, 15);
            */

            //array test


            /*
            content.SetColorStroke(BaseColor.BLUE);
            content.MoveTo(72.0f, PageSize.A4.Height - 10);
            content.LineTo(300.0f, PageSize.A4.Height - 10);
            content.Stroke();
            */
            /*
            content.SetColorFill(BaseColor.PINK);
            content.Ellipse(100, 100, 300, 200);
            content.Fill();
            */
            /*
            content.SetColorStroke(BaseColor.RED);
            content.Rectangle(PageSize.A4.Width / 7, PageSize.A4.Height * 3 / 4, PageSize.A4.Width / 2, PageSize.A4.Height / 5);
            content.Stroke();
            */

            //close
            document.Close();
        }

        static void DrawOneScore(PdfContentByte content, float left, float up, float number, bool flag = true)
        {
            up = PageSize.A4.Height - up;
            float position = up - 5 * lineSpace + (number - 1) * lineSpace / 2;
            //circle
            content.SetColorFill(BaseColor.BLACK);
            content.Circle(left, position, scoreRadius);
            content.Fill();
            //vertical line
            if (number < 8)
            {
                content.MoveTo(left + scoreRadius, position);
                content.LineTo(left + scoreRadius, position + 3 * lineSpace);
                content.Stroke();
            }
            else
            {
                content.MoveTo(left - scoreRadius, position);
                content.LineTo(left - scoreRadius, position - 3 * lineSpace);
                content.Stroke();
            }
            //if need addition lateral line
            if(number<2 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position + scoreRadius);
                content.LineTo(left + scoreRadius * 2, position + scoreRadius);
                content.Stroke();
            }
            else if(number<2 && ((int)number % 2 != 0))
            {
                content.MoveTo(left - scoreRadius * 2, position);
                content.LineTo(left + scoreRadius * 2, position);
                content.Stroke();
            }
            else if(number>12 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position - scoreRadius);
                content.LineTo(left + scoreRadius * 2, position - scoreRadius);
                content.Stroke();
            }
            else if(number>12 && ((int)number % 2 != 0))
            {
                content.MoveTo(left - scoreRadius * 2, position);
                content.LineTo(left + scoreRadius * 2, position);
                content.Stroke();
            }
            //if need half upward
            if (!flag)
            {

            }
        }

        static void DrawFiveLines(PdfContentByte content, float left, float right, float up, float height = lineSpace)
        {
            up = PageSize.A4.Height - up;
            content.SetColorStroke(BaseColor.BLACK);
            //five lateral lines
            content.MoveTo(left, up);
            content.LineTo(right, up);
            content.MoveTo(left, up - height);
            content.LineTo(right, up - height);
            content.MoveTo(left, up - height * 2);
            content.LineTo(right, up - height * 2);
            content.MoveTo(left, up - height * 3);
            content.LineTo(right, up - height * 3);
            content.MoveTo(left, up - height * 4);
            content.LineTo(right, up - height * 4);
            //two vertical lines
            content.MoveTo(left, up);
            content.LineTo(left, up - height * 4);
            content.MoveTo(left + headSpace, up);
            content.LineTo(left + headSpace, up - height * 4);
            content.MoveTo(right, up);
            content.LineTo(right, up - height * 4);
            content.MoveTo(right - headSpace, up);
            content.LineTo(right - headSpace, up - height * 4);
            //draw
            content.Stroke();
        }
    }
}
