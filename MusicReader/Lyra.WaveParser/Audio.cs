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
        /// <param name="filename">file name of audio file</param>
        public Audio(string filename)
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
            for (int i = 0; i < length; ++i)
            {
                data[i] = tmpData[i * sampleScale];
            }
        }
    }
}
