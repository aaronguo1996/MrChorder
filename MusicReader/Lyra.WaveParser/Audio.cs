namespace Lyra.WaveParser
{
    using System;
    using AForge.Math;
    using NAudio.Wave;

    /// <summary>
    /// audio file reader
    /// </summary>
    public class Audio
    {
        /// <summary>
        /// chunk data of wav file
        /// </summary>
        public byte[] data { get; set; }

        /// <summary>
        /// frequency of sampling
        /// </summary>
        public int fs { get; set; }

        /// <summary>
        /// count of sample point to do fft
        /// </summary>
        public int fftLength { get; set; }

        /// <summary>
        /// Error of audio file
        /// </summary>
        private AUDIO_ERROR Err = AUDIO_ERROR.NONE;

        /// <summary>
        /// max value of int
        /// </summary>
        private const long MAX_INT = 2147483647;

        /// <summary>
        /// min frequency to recognize
        /// </summary>
        private const int MIN_FS = 4;

        /// <summary>
        /// max frequency to recognize
        /// </summary>
        private const int MAX_FS = 4096;

        /// <summary>
        /// read wav file
        /// </summary>
        /// <param name="filename">file name of audio file</param>
        public Audio(string filename)
        {
            try
            {
                //read music file, only wav for now
                if (!filename.EndsWith(".wav"))
                {
                    Err = AUDIO_ERROR.UNKNOWN_FILE_FORMAT;
                    return;
                }

                var fileReader = new WaveFileReader(filename);
                var wavStream = WaveFormatConversionStream.CreatePcmStream(fileReader);

                //cannot deal with audio too long(count of sampling points bigger than MAX_INT)
                if (wavStream.Length > MAX_INT)
                {
                    Err = AUDIO_ERROR.AUDIO_TOO_LONG;
                    return;
                }
                //cannot deal with audio too short(time smaller than 500 ms)
                else if (wavStream.TotalTime.TotalMilliseconds < 500)
                {
                    Err = AUDIO_ERROR.AUDIO_TOO_SHORT;
                    return;
                }

                //read raw data
                int rawLength = (int)wavStream.Length;
                double rawFs = rawLength / wavStream.TotalTime.TotalSeconds;
                if (rawFs < MAX_FS * 2)
                {
                    Err = AUDIO_ERROR.AUDIO_SAMPLE_NOT_ENOUGH;
                    return;
                }

                byte[] rawData = new byte[rawLength];
                wavStream.Read(rawData, 0, rawLength);

                //down sampling
                this.fs = MAX_FS * 2;
                this.fftLength = this.fs / 4;
                double scale = rawFs / this.fs;
                int length = (int)(rawLength / scale);
                this.data = new byte[length];
                for (int i = 0; i < length; ++i)
                {
                    int startIndex = (int)(scale * i);
                    int endIndex = (int)(scale * (i + 1));
                    
                    /*
                    //average value filter
                    int result = 0;
                    for (int j = startIndex; j < endIndex; ++j)
                    {
                        result += rawData[j];
                    }

                    this.data[i] = (byte)(result / (endIndex - startIndex));
                    */
                    
                    //mid value filter
                    Array.Sort(rawData, startIndex, endIndex - startIndex);
                    this.data[i] = rawData[(startIndex + endIndex) / 2];
                    
                }
            }
            catch
            {
                Err = AUDIO_ERROR.INTERNAL_ERROR;
                return;
            }
        }

        public float[][] GetNMaxAmpFreqs(int n)
        {
            //count is 32 magically
            //[TODO] n is 5 magically, and freq start from 60
            //const int count = 1000;
            const int count = 32;
            n = 5;
            float[][] result = new float[count][];
            double[] fftData;
            int offset = 0;
            //for (int i = 0; i < count && offset < this.data.Length - this.fftLength; ++i, offset += this.fftLength / 25)
            for (int i = 0; i < count; ++i, offset += 128)
            {
                fftData = GetFFTResult(offset);
                while (true)
                {
                    Array.Sort(fftData, 15, 5);
                    int max1AmplitudeIndex = 19;
                    int max2AmplitudeIndex = 18;
                    int max3AmplitudeIndex = 17;
                    int max4AmplitudeIndex = 16;
                    int max5AmplitudeIndex = 15;

                    for (int j = n + 15; j < this.fftLength / 2; ++j)
                    {
                        if (fftData[j] > fftData[max1AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = max2AmplitudeIndex;
                            max2AmplitudeIndex = max1AmplitudeIndex;
                            max1AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max2AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = max2AmplitudeIndex;
                            max2AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max3AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = max3AmplitudeIndex;
                            max3AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max4AmplitudeIndex])
                        {
                            max5AmplitudeIndex = max4AmplitudeIndex;
                            max4AmplitudeIndex = j;
                        }
                        else if (fftData[j] > fftData[max5AmplitudeIndex])
                        {
                            max5AmplitudeIndex = j;
                        }
                    }

                    int[] tmpIndices = new int[n];
                    tmpIndices[0] = max1AmplitudeIndex;
                    tmpIndices[1] = max2AmplitudeIndex;
                    tmpIndices[2] = max3AmplitudeIndex;
                    tmpIndices[3] = max4AmplitudeIndex;
                    tmpIndices[4] = max5AmplitudeIndex;
                    Array.Sort(tmpIndices);

                    bool passed = true;
                    //elinimate near frequency
                    for (int j = 1; j < n; ++j)
                    {
                        if (tmpIndices[j] <= tmpIndices[j - 1] + 5)
                        {
                            fftData[tmpIndices[j]] = 0;
                            passed = false;
                        }
                    }

                    if (passed)
                    {
                        result[i] = new float[n];
                        for (int j = 0; j < n; ++j)
                        {
                            result[i][j] = (float)tmpIndices[j] * this.fs / this.fftLength;
                        }

                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// get fft result after index, use 0.25s
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public double[] GetFFTResult(int index)
        {
            //0.25s
            Complex[] fftData = new Complex[this.fftLength];
            double[] result = new double[this.fftLength];
            for (int i = 0; i < this.fftLength; ++i)
            {
                fftData[i] = new Complex(this.data[index + i], 0);
            }

            FourierTransform.FFT(fftData, FourierTransform.Direction.Forward);
            for (int i = 0; i < this.fftLength / 2; ++i)
            {
                result[i] = fftData[i].Re * fftData[i].Re + fftData[i].Im * fftData[i].Im;
            }

            return result;
        }

        /// <summary>
        /// get result notes from audio
        /// </summary>
        /// <returns></returns>
        public float[] GetNotes()
        {
            //[TODO]
            if(Err != AUDIO_ERROR.NONE)
            {
                return null;
            }

            return null;
        }

        public string GetError()
        {
            switch (Err)
            {
                case AUDIO_ERROR.NONE:
                    return "";
                case AUDIO_ERROR.AUDIO_TOO_LONG:
                    return "Audio too long.";
                case AUDIO_ERROR.AUDIO_TOO_SHORT:
                    return "Audio too short.";
                case AUDIO_ERROR.UNKNOWN_FILE_FORMAT:
                    return "Unknown file format.";
                case AUDIO_ERROR.AUDIO_SAMPLE_NOT_ENOUGH:
                    return "Audio sampling frequency not enough";
                default:
                    return "Internal error.";
            }
        }
    }
    
        

    internal enum AUDIO_ERROR { NONE, UNKNOWN_FILE_FORMAT, AUDIO_TOO_LONG, AUDIO_TOO_SHORT, AUDIO_SAMPLE_NOT_ENOUGH, INTERNAL_ERROR};
}
