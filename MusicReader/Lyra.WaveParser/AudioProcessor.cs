namespace Lyra.WaveParser
{
    using AForge.Math;

    public class AudioProcessor
    {
        public static void getMax3Amp(Audio audio, int length, int markLength, ref int amp1, ref int amp2, ref int amp3)
        {
            Complex[] fftData = new Complex[length];
            for (int i = 0; i < length; ++i)
            {
                fftData[i] = new Complex(audio.data[markLength + i], 0);
            }

            FourierTransform.DFT(fftData, FourierTransform.Direction.Forward);

            //get max three freq
            fftData[3].Re = fftData[3].Re * fftData[3].Re + fftData[3].Im * fftData[3].Im;
            fftData[1].Re = fftData[1].Re * fftData[1].Re + fftData[1].Im * fftData[1].Im;
            fftData[2].Re = fftData[2].Re * fftData[2].Re + fftData[2].Im * fftData[2].Im;
            int max1AmplitudeIndex = 0;
            int max2AmplitudeIndex = 0;
            int max3AmplitudeIndex = 0;

            if (fftData[3].Re >= fftData[1].Re && fftData[3].Re >= fftData[2].Re)
            {
                max1AmplitudeIndex = 3;
                //12
                if (fftData[1].Re >= fftData[2].Re)
                {
                    max2AmplitudeIndex = 1;
                    max3AmplitudeIndex = 2;
                }
                else
                {
                    max2AmplitudeIndex = 2;
                    max3AmplitudeIndex = 1;
                }
            }
            else if (fftData[1].Re >= fftData[3].Re && fftData[1].Re >= fftData[2].Re)
            {
                max1AmplitudeIndex = 1;
                //32
                if (fftData[3].Re >= fftData[2].Re)
                {
                    max2AmplitudeIndex = 3;
                    max3AmplitudeIndex = 2;
                }
                else
                {
                    max2AmplitudeIndex = 2;
                    max3AmplitudeIndex = 3;
                }
            }
            else if (fftData[2].Re >= fftData[3].Re && fftData[2].Re >= fftData[1].Re)
            {
                max1AmplitudeIndex = 2;
                //31
                if (fftData[3].Re >= fftData[1].Re)
                {
                    max2AmplitudeIndex = 3;
                    max3AmplitudeIndex = 1;
                }
                else
                {
                    max2AmplitudeIndex = 1;
                    max3AmplitudeIndex = 3;
                }
            }
            else
            {
                return;
            }

            for (int i = 4; i < length / 2; ++i)
            {
                fftData[i].Re = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
                if (fftData[i].Re > fftData[max1AmplitudeIndex].Re)
                {
                    max3AmplitudeIndex = max2AmplitudeIndex;
                    max2AmplitudeIndex = max1AmplitudeIndex;
                    max1AmplitudeIndex = i;
                }
                else if (fftData[i].Re > fftData[max2AmplitudeIndex].Re)
                {
                    max3AmplitudeIndex = max2AmplitudeIndex;
                    max2AmplitudeIndex = i;
                }
                else if (fftData[i].Re > fftData[max3AmplitudeIndex].Re)
                {
                    max3AmplitudeIndex = i;
                }
            }
        }
    }
}
