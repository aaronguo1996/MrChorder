using System;
using System.Drawing;
using System.Threading;
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
            int lockedLength = 100;
            WavFile wavFile = new WavFile("t.wav");
            int length = (wavFile.length < lockedLength) ? wavFile.length : lockedLength;

            int markLength = 0;
            while (markLength < wavFile.length - lockedLength)
            {
                e.Graphics.Clear(Color.White);
                //amplitude
                Pen blackPen = new Pen(Color.Black, 1);
                double gap = 1000.0 / length;
                Point p1;
                Point p2 = new Point(20, 300 - wavFile.data[markLength]);
                for (int i = 1; i < length; ++i)
                {
                    p1 = p2;
                    p2 = new Point(20 + (int)(i * gap), 300 - wavFile.data[markLength + i]);
                    e.Graphics.DrawLine(blackPen, p1, p2);
                }

                //fft
                Complex[] fftData = new Complex[length];
                for (int i = 0; i < length; ++i)
                {
                    fftData[i] = new Complex(wavFile.data[markLength + i], 0);
                }

                FourierTransform.DFT(fftData, FourierTransform.Direction.Forward);

                int maxAmplitudeIndex = 0;
                double maxAmplitude = 0;
                for (int i = 0; i < length; ++i)
                {
                    fftData[i].Re = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
                    if(fftData[i].Re > maxAmplitude)
                    {
                        maxAmplitude = fftData[i].Re;
                        maxAmplitudeIndex = i;
                    }
                }

                double fftGraphScale = 256 / maxAmplitude;

                e.Graphics.DrawLine(blackPen, new Point(0, 600), new Point(1024, 600));
                for (int i = 0; i < length; ++i)
                {
                    p1 = new Point((int)(20 + i * gap), 600);
                    p2 = new Point((int)(20 + i * gap), (int)(600 - fftData[i].Re * fftGraphScale));
                    e.Graphics.DrawLine(blackPen, p1, p2);
                }

                //loop variable
                markLength += lockedLength;
                Thread.Sleep(2000);
            }
        }
    }
}
