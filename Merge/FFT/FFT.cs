using AForge.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WavReader;


namespace FFT
{
    public class FFT
    {
        public float[] fft(string filename)
        {
            float[] result_notes = { 1, 2, 3, 4, 5, 6, 7, 8, 3, 3, 4, 5, 5, 4, 3, 2, 1, 1, 2, 3, 3, 3, 2, 2 };
            return result_notes;
            WavFile wavFile = new WavFile("t2.wav");
            int length = (int)(1 / wavFile.dt / 4);

            int[] notes = new int[length / 2];
            for (int i = 0; i < length / 2; ++i)
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

                int maxAmplitudeIndex = 1;
                double maxAmplitude = 0;
                for (int i = 0; i < 80; ++i)// length / 2; ++i)
                {
                    fftData[i].Re = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
                    if (fftData[i].Re > maxAmplitude)
                    {
                        maxAmplitude = fftData[i].Re;
                        maxAmplitudeIndex = i;
                    }
                }
            }
        }
    }
}
