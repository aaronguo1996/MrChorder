using NAudio.Wave;

/// <summary>
/// read wav file
/// </summary>
namespace WavReader
{
    public class WavFile
    {
        //chunk data of wav file
        public byte[] data { get; set; }

        //length of data
        public int length { get; set; }

        /// <summary>
        /// read wav file
        /// </summary>
        /// <param name="filename">wav file name</param>
        public WavFile(string filename)
        {
            var fileReader = new WaveFileReader(filename);
            var wavStream = WaveFormatConversionStream.CreatePcmStream(fileReader);
            length = (int)wavStream.Length;
            data = new byte[length];
            wavStream.Read(data, 0, length);
        }
    }
}
