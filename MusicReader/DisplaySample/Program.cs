using System;
using System.Drawing;
using System.Windows.Forms;
using WavReader;

namespace DisplaySample
{
    static class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new Form();

            form.Paint += new PaintEventHandler(paintHandler);
            Application.Run(form);
        }

        public static void paintHandler(Object sender, PaintEventArgs e)
        {
            WavFile wavFile = new WavFile("t.wav");
            int length = (wavFile.length < 1000) ? wavFile.length : 1000;
            Pen blackPen = new Pen(Color.Black, 1);
            double gap = 1000.0 / length;
            Point p1 = new Point(0, 303 - wavFile.data[0]);
            Point p2 = p1;
            for (int i = 1; i < length; ++i)
            {
                p1 = p2;
                p2 = new Point((int)(i * gap), 303 - wavFile.data[i]);
                e.Graphics.DrawLine(blackPen, p1, p2);
            }
        }
    }
}
