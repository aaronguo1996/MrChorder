using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace PDF
{
    public class ToPDF
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
        static private string imagePath;
        public void GeneratePDF(string imgPath, string path, float[] testMusic, int size)
        {
            imagePath = imgPath;
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(path, FileMode.OpenOrCreate));

            //Console.Write("%f %f", PageSize.A4.Height, PageSize.A4.Width);
            //open
            document.Open();

            //hello world
            document.AddTitle("Hello World");
            document.AddSubject("Music Score");
            //842 595
            Paragraph musicName = new Paragraph("Name of Music");
            musicName.Alignment = Element.ALIGN_CENTER;
            document.Add(musicName);

            //draw
            PdfContentByte content = writer.DirectContent;

            //array test [TODO]
            /*float[] testMusic;
            testMusic = new float[50];
            for(int n = 0; n < 50; n++)
            {
                testMusic[n] = n % 20 - 4;
            }
            int size = 50;*/
            DrawFromArray(content, testMusic, size);

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
            //it is moderate
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED), 10);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - beginHeight + 2 * lineSpace);
            content.ShowText("Moderate        =120");
            content.EndText();
            DrawOneScore(content, beginLeft + 14 * headSpace, beginHeight - 5.5f * lineSpace, 2);
            //draw the sign
            //Image sign = Image.GetInstance("F:\\Microsoft\\MrChorder\\sign.png");
            //sign.SetAbsolutePosition(beginLeft - 18, PageSize.A4.Height - (beginHeight + 5 * lineSpace));
            //sign.ScaleAbsoluteHeight(6 * lineSpace);
            //sign.ScaleAbsoluteWidth(3.5f * lineSpace);
            //content.AddImage(sign);
            //draw the tempo
            content.BeginText();
            content.SetFontAndSize(BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED), 16);
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 2 * lineSpace));
            content.ShowText("4");
            content.SetTextMatrix(beginLeft, PageSize.A4.Height - (beginHeight + 4 * lineSpace));
            content.ShowText("4");
            content.EndText();
            //draw the scores
            for (int i = 0; i < size; i++)
            {
                //new line
                if (i % ((int)count * tempo) == 0)
                {
                    line++;
                    //draw the sign
                    Image sign = Image.GetInstance(imagePath);
                    sign.SetAbsolutePosition(beginLeft - 18, PageSize.A4.Height - (beginHeight + line * intervalHeight + 5 * lineSpace));
                    sign.ScaleAbsoluteHeight(6 * lineSpace);
                    sign.ScaleAbsoluteWidth(3.5f * lineSpace);
                    content.AddImage(sign);
                    //DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight, count);
                    if (size - i <= count * tempo)
                    {
                        int remain = (size - i) % (int)tempo == 0 ? (size - i) / (int)tempo : (size - i) / (int)tempo + 1;
                        DrawFiveLines(content, beginLeft, beginLeft + ((float)remain / (float)count) * (endRight - beginLeft), beginHeight + line * intervalHeight, remain);
                        //end vertical line
                        content.MoveTo(beginLeft + ((float)remain / (float)count) * (endRight - beginLeft) - headSpace, PageSize.A4.Height - (beginHeight + line * intervalHeight));
                        content.LineTo(beginLeft + ((float)remain / (float)count) * (endRight - beginLeft) - headSpace, PageSize.A4.Height - (beginHeight + line * intervalHeight) - lineSpace * 4);
                        content.Stroke();
                    }
                    else
                    {
                        DrawFiveLines(content, beginLeft, endRight, beginHeight + line * intervalHeight, count);
                    }
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
            if (number < 2 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position + scoreRadius);
                content.LineTo(left + scoreRadius * 2, position + scoreRadius);
                content.Stroke();
            }
            else if (number < 2 && ((int)number % 2 != 0))
            {
                content.MoveTo(left - scoreRadius * 2, position);
                content.LineTo(left + scoreRadius * 2, position);
                content.Stroke();
            }
            else if (number > 12 && ((int)number % 2 == 0))
            {
                content.MoveTo(left - scoreRadius * 2, position - scoreRadius);
                content.LineTo(left + scoreRadius * 2, position - scoreRadius);
                content.Stroke();
            }
            else if (number > 12 && ((int)number % 2 != 0))
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
            content.MoveTo(right, up);
            content.LineTo(right, up - height * 4);
            //middle lines
            float width = (right - left) / count;
            for (int i = 1; i < count; i++)
            {
                content.MoveTo(left + width * i, up);
                content.LineTo(left + width * i, up - height * 4);
            }
            //draw
            content.Stroke();
        }
    }
}
