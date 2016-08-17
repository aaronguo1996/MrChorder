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
                double scale = rawFs / this.fs;
                int length = (int)(rawLength / scale);
                this.data = new byte[length];
                for (int i = 0; i < length; ++i)
                {
                    //average value filter
                    int result = 0;
                    int startIndex = (int)(scale * i);
                    int endIndex = (int)(scale * (i + 1));
                    for (int j = startIndex; j < endIndex; ++j)
                    {
                        result += rawData[j];
                    }

                    this.data[i] = (byte)(result / (endIndex - startIndex));
                }
            }
            catch
            {
                Err = AUDIO_ERROR.INTERNAL_ERROR;
                return;
            }
        }

        public float[][] get3MaxAmpFreqs(int count)
        {
            //[TODO] count is 16 magically, and should add time selection support
            float[][] result = new float[count][];

            //0.25s
            int fftLength = this.fs / 4;
            Complex[] fftData = new Complex[fftLength];

            int offset = 4096;
            for (int i = 0; i < count; ++i, offset += 512)
            {
                for (int j = 0; j < fftLength; ++j) {
                    fftData[j] = new Complex(this.data[offset + j], 0);
                }

                FourierTransform.FFT(fftData, FourierTransform.Direction.Forward);
                for (int j = 0; j < fftLength / 2; ++j)
                {
                    fftData[j].Re = fftData[j].Re * fftData[j].Re + fftData[j].Im * fftData[j].Im;
                }

                //find 3 frequencies with max amplitude
                //[TODO] fix this
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
                    throw new Exception("xxxxx");
                }


                for (int j = 4; j < fftLength / 2; ++j)
                {
                    fftData[j].Re = fftData[j].Re * fftData[j].Re + fftData[j].Im * fftData[j].Im;
                    if (fftData[j].Re > fftData[max1AmplitudeIndex].Re)
                    {
                        max3AmplitudeIndex = max2AmplitudeIndex;
                        max2AmplitudeIndex = max1AmplitudeIndex;
                        max1AmplitudeIndex = j;
                    }
                    else if (fftData[j].Re > fftData[max2AmplitudeIndex].Re)
                    {
                        max3AmplitudeIndex = max2AmplitudeIndex;
                        max2AmplitudeIndex = j;
                    }
                    else if (fftData[j].Re > fftData[max3AmplitudeIndex].Re)
                    {
                        max3AmplitudeIndex = j;
                    }
                }

                result[i] = new float[3];
                result[i][0] = (float)max1AmplitudeIndex * this.fs / fftLength;
                result[i][1] = (float)max2AmplitudeIndex * this.fs / fftLength;
                result[i][2] = (float)max3AmplitudeIndex * this.fs / fftLength;
                if(result[i][1] < result[i][2])
                {
                    float tmp = result[i][1];
                    result[i][1] = result[i][2];
                    result[i][2] = tmp;
                }
                if (result[i][0] < result[i][1])
                {
                    float tmp = result[i][0];
                    result[i][0] = result[i][1];
                    result[i][1] = tmp;
                }
                if (result[i][1] < result[i][2])
                {
                    float tmp = result[i][1];
                    result[i][1] = result[i][2];
                    result[i][2] = tmp;
                }
            }

            return result;
        }

        /// <summary>
        /// get result notes from audio
        /// </summary>
        /// <returns></returns>
        public float[] GetNotes()
        {
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
