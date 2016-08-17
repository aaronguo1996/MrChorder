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
        }
    }
}
