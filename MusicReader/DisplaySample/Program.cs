using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Math;
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
            int lockedLength = 50;
            WavFile wavFile = new WavFile("t.wav");
            int length = (wavFile.length < lockedLength) ? wavFile.length : lockedLength;

            //amplitude
            Pen blackPen = new Pen(Color.Black, 1);
            double gap = 1000.0 / length;
            Point p1;
            Point p2 = new Point(20, 400 - wavFile.data[0]);
            for (int i = 1; i < length; ++i)
            {
                p1 = p2;
                p2 = new Point(20 + (int)(i * gap), 400 - wavFile.data[i]);
                e.Graphics.DrawLine(blackPen, p1, p2);
            }
                       
            //fft
            Complex[] fftData = new Complex[length];
            for (int i = 0; i < length; ++i)
            {
                fftData[i] = new Complex(wavFile.data[i], 0);
            }
            FourierTransform.DFT(fftData, FourierTransform.Direction.Forward);


            e.Graphics.DrawLine(blackPen, new Point(0, 500), new Point(1024, 500));
            for (int i = 0; i < length; ++i)
            {
                p1 = new Point((int)(300 + fftData[i].Re * 20), 500);
                p2 = new Point((int)(300 + fftData[i].Re * 20), 500 - (int)fftData[i].Im);
                e.Graphics.DrawLine(blackPen, p1, p2);
            }

            
        }
    }
}
