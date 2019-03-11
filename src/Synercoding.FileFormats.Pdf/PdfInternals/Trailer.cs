using Synercoding.FileFormats.Pdf.Extensions;
using Synercoding.FileFormats.Pdf.Helpers;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals
{
    internal struct Trailer : IStreamWriteable
    {
        public Trailer(uint startXRef, int size, PdfReference root, PdfReference documentInfo)
        {
            StartXRef = startXRef;
            Size = size;
            Root = root;
            DocumentInfo = documentInfo;
        }

        public uint StartXRef { get; }
        public int Size { get; }
        public PdfReference Root { get; }
        public PdfReference DocumentInfo { get; }

        public uint WriteToStream(Stream stream)
        {
            var position = (uint)stream.Position;

            var size = Size;
            var root = Root;
            var info = DocumentInfo;

            stream
                .Write("trailer")
                .NewLine()
                .Dictionary(dictionary =>
                {
                    dictionary
                        .Write("/Size", size)
                        .Write("/Root", root)
                        .Write("/Info", info);
                })
                .Write("startxref")
                .NewLine()
                .Write(StartXRef)
                .NewLine()
                .Write("%%EOF");

            return position;
        }
    }
}