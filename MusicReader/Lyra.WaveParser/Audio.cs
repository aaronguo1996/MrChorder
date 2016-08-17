namespace Lyra.WaveParser
{
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
                for(int i = 0; i < length; ++i)
                {
                    //middle value sampling
                    
                }
            }
            catch
            {
                Err = AUDIO_ERROR.INTERNAL_ERROR;
                return;
            }
        }
        public float[] GetNotes()
        {
            if (Err != AUDIO_ERROR.NONE)
            {
                return null;
            }

            return new float[3] { 1, 2, 3 };
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
