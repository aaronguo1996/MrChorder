using System;
using System.Drawing;
using System.IO;
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
            StreamWriter writer = new StreamWriter("amplitude.txt", false);
            writer.WriteLine("frequency1: amplitude1   ;   frequency2: amplitude2");
            writer.Close();
            WavFile wavFile = new WavFile("t.wav");
            int length = (int)(1 / wavFile.dt / 4);
            Pen blackPen = new Pen(Color.Black, 1);
            Pen redPen = new Pen(Color.Red, 1);

            double gap = 1000.0 / length;
            Point p1;
            Point p2;

            int[] notes = new int[length / 2];
            for(int i = 0; i < length / 2; ++i)
            {
                notes[i] = 0;
            }
            int[] results = new int[wavFile.length / length / 4 * 8];
            int result_index = 0;
            int markLength = 0;
            while (markLength < wavFile.length - length)
            {
                //fft
                Complex[] fftData = new Complex[length];
                for (int i = 0; i < length; ++i)
                {
                    fftData[i] = new Complex(wavFile.data[markLength + i], 0);
                }

                FourierTransform.DFT(fftData, FourierTransform.Direction.Forward);


                fftData[0].Re = fftData[0].Re * fftData[0].Re + fftData[0].Im * fftData[0].Im;
                fftData[1].Re = fftData[1].Re * fftData[1].Re + fftData[1].Im * fftData[1].Im;
                int maxAmplitudeIndex = 0;
                int max2AmplitudeIndex = 0;
                if(fftData[0].Re > fftData[1].Re)
                {
                    max2AmplitudeIndex = 1;
                }else
                {
                    maxAmplitudeIndex = 1;
                }

                double maxAmplitude = 0;
                for (int i = 2; i < 80; ++i)// length / 2; ++i)
                {
                    fftData[i].Re = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
                    if(fftData[i].Re > maxAmplitude)
                    {
                        maxAmplitude = fftData[i].Re;
                        max2AmplitudeIndex = maxAmplitudeIndex;
                        maxAmplitudeIndex = i;
                    }
                }

                double max2Amplitude = fftData[max2AmplitudeIndex].Re;

                double frequency = 1 / wavFile.dt * max2AmplitudeIndex / length;

                writer = new StreamWriter("amplitude.txt",true);
                writer.WriteLine($"{1 / wavFile.dt * maxAmplitudeIndex / length}: {maxAmplitude}   ;   {1 / wavFile.dt * max2AmplitudeIndex / length}: {max2Amplitude}");
                writer.Close();

                double fftGraphScale = 0.02;

                e.Graphics.DrawLine(blackPen, new Point(0, 600), new Point(1024, 600));
                for (int i = 20; i < 80; ++i)//length / 2 ; ++i)
                {
                    if (fftData[i].Re > 60)
                    {
                      //  p1 = new Point((int)(20 + i * gap * 2), 600);
                       // p2 = new Point((int)(20 + i * gap * 2), (int)(600 - fftData[i].Re * fftGraphScale));
                      //  e.Graphics.DrawLine(blackPen, p1, p2);
                        notes[i]++;
                    }
                }

                for(int i = 0; i < length / 2 ; ++i)
                {
                    p1 = new Point((int)(20 + i * gap * 2), 600);
                    p2 = new Point((int)(20 + i * gap * 2), (int)(600 - notes[i] * 20));
                    e.Graphics.DrawLine(blackPen, p1, p2);
                }

                e.Graphics.DrawLine(redPen, new Point((int)(261 * wavFile.dt * 1000 + 20), 600), new Point((int)(261 * wavFile.dt * 1000 + 20), 900));
                e.Graphics.DrawLine(redPen, new Point((int)(493.88 * wavFile.dt * 1000 + 20), 600), new Point((int)(493.88 * wavFile.dt * 1000 + 20), 900));

                //e.Graphics.DrawLine(redPen, new Point((int)(784 * wavFile.dt * 1000 + 20), 0), new Point((int)(784 * wavFile.dt * 1000 + 20), 900));
               // e.Graphics.DrawLine(redPen, new Point((int)(830 * wavFile.dt * 1000 + 20), 0), new Point((int)(830 * wavFile.dt * 1000 + 20), 900));
               // e.Graphics.DrawLine(redPen, new Point((int)(987.77 * wavFile.dt * 1000 + 20), 0), new Point((int)(987.77 * wavFile.dt * 1000 + 20), 900));


                //loop variable
                markLength += length;
                if(markLength >= length / 2 * (result_index + 1))
                {
                    int max_note = 0;
                    int max_note_index = 0;
                    for(int note_i = 0; note_i < 80; ++note_i)//length / 2; ++note_i)
                    {
                        if(notes[note_i] > max_note)
                        {
                            max_note = notes[note_i];
                            max_note_index = note_i;
                        }
                    }
                    results[result_index] = max_note_index;
                    result_index++;
                    for (int i = 0; i < length / 2; ++i)
                    {
                        notes[i] = 0;
                    }
                }
            }
        }
    }
}
