using NAudio.Wave;

/// <summary>
/// read wav file
/// </summary>
namespace WavReader
{
    public class WavFile
    {
        /// <summary>
        /// chunk data of wav file
        /// </summary>
        public byte[] data { get; set; }

        /// <summary>
        /// length of data
        /// </summary>
        public int length { get; set; }

        /// <summary>
        /// delta time between two sample point (second)
        /// </summary>
        public double dt { get; set; }

        /// <summary>
        /// read wav file
        /// </summary>
        /// <param name="filename">wav file name</param>
        public WavFile(string filename)
        {
            int sampleScale = 100;
            var fileReader = new WaveFileReader(filename);
            var wavStream = WaveFormatConversionStream.CreatePcmStream(fileReader);
            length = (int)wavStream.Length;
            byte[] tmpData = new byte[length];
            dt = wavStream.TotalTime.TotalSeconds / length * sampleScale;
            wavStream.Read(tmpData, 0, length);
            data = new byte[length / sampleScale];
            length = length / sampleScale;
            for(int i = 0; i < length; ++i)
            {
                data[i] = tmpData[i * sampleScale];
            }
        }
    }
}
