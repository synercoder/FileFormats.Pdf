using System.IO;

namespace Synercoding.FileFormats.Pdf.Helpers
{
    public interface IStreamWriteable
    {
        /// <summary>
        /// Write the object to the provided stream
        /// </summary>
        /// <param name="stream">The stream to write to</param>
        /// <returns>The position in the stream where is object is written</returns>
        uint WriteToStream(Stream stream);
    }
}