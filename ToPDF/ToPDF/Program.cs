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
        const float defaultTempo = 4;
        const int defaultCount = 4;
        const float defaultBeginHeight = 100;
        const float defaultIntervalHeight = 80;
        const float defaultBeginLeft = 100;
        const float defaultEndRight = 500;
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

            //array test
            float[] testMusic = { 3, 3, 4, 5, 5, 4, 3, 2, 1, 1, 2, 3, 3, 2, 2, -2, -3, 16, 17, 4, 6, 9 };
            int size = 22;
            DrawFromArray(content, testMusic, size);


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

        /* draw the score from an array
         * content: drawing object
         * musicArray: source data
         * size: data size
         * count: number of bars in one line
         * tempo: number of notes in one bar
         */
        static void DrawFromArray(PdfContentByte content, float[] musicArray, int size, int count = defaultCount, float tempo = defaultTempo)
        {
            float beginHeight = defaultBeginHeight;
            float beginLeft = defaultBeginLeft;
            float endRight = defaultEndRight;
            float intervalHeight = defaultIntervalHeight;
            int line = -1;
            int c = -1;
            float width = (endRight - beginLeft) / count;
            float widthScore = (endRight - beginLeft) / (count * (tempo + 1));
            for (int i = 0; i < size; i++)
            {
                //new line
                if (i % ((int)count * tempo) == 0)
                {
                    line++;
                    DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight, count);
                    Image sign = Image.GetInstance("C:\\Users\\t-yaxie\\Desktop\\Fun\\MrChorder\\sign.png");
                    sign.SetAbsolutePosition(beginLeft - 12, PageSize.A4.Height - (beginHeight + line * intervalHeight + 5 * lineSpace));
                    //sign.ScalePercent(8);
                    sign.ScaleAbsoluteHeight(6 * lineSpace);
                    sign.ScaleAbsoluteWidth(4 * lineSpace);
                    content.AddImage(sign);
                }
                //switch to next bar
                if (i % ((int)tempo) == 0)
                {
                    c = (c + 1) % count;
                }
                DrawOneScore(content, beginLeft + c * width + widthScore * (i % ((int)tempo) + 1), beginHeight + line * intervalHeight, musicArray[i]);
            }
        }

        /* draw a note
         * content: drawing object
         * left: beginning position
         * up: beginning position
         * number: tone
         * flag: whether needs half tone
         */
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

        /* draw five lines
         * content: drawing object
         * left: beginning position
         * right: ending position
         * up: beginning position
         * count: number of bars
         * height: space between lines
         */ 
        static void DrawFiveLines(PdfContentByte content, float left, float right, float up, int count, float height = lineSpace)
        {
            up = PageSize.A4.Height - up;
            content.SetColorStroke(BaseColor.BLACK);
            //five lateral lines
            content.MoveTo(left - 20, up);
            content.LineTo(right, up);
            content.MoveTo(left - 20, up - height);
            content.LineTo(right, up - height);
            content.MoveTo(left - 20, up - height * 2);
            content.LineTo(right, up - height * 2);
            content.MoveTo(left - 20, up - height * 3);
            content.LineTo(right, up - height * 3);
            content.MoveTo(left - 20, up - height * 4);
            content.LineTo(right, up - height * 4);
            //start & end vertical lines
            content.MoveTo(left - 20, up);
            content.LineTo(left - 20, up - height * 4);
            content.MoveTo(left - 20 + headSpace, up);
            content.LineTo(left - 20 + headSpace, up - height * 4);
            content.MoveTo(right, up);
            content.LineTo(right, up - height * 4);
            content.MoveTo(right - headSpace, up);
            content.LineTo(right - headSpace, up - height * 4);
            //middle lines
            float width = (right - left) / count;
            for(int i = 1; i < count; i++)
            {
                content.MoveTo(left + width * i, up);
                content.LineTo(left + width * i, up - height * 4);
            }
            //draw
            content.Stroke();
        }
    }
}
