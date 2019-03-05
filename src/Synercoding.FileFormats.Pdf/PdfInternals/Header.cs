using Synercoding.FileFormats.Pdf.Helpers;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals
{
    internal struct Header : ISpanWriteable, IStreamWriteable
    {
        public int ByteSize()
        {
            return 17;
        }

        public void FillSpan(Span<byte> bytes)
        {
            var position = 0;
            bytes[position++] = 0x25; // %
            bytes[position++] = 0x50; // P
            bytes[position++] = 0x44; // D
            bytes[position++] = 0x46; // F
            bytes[position++] = 0x2D; // -
            bytes[position++] = 0x31; // 1
            bytes[position++] = 0x2E; // .
            bytes[position++] = 0x37; // 7
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
            bytes[position++] = 0x25; // %
            bytes[position++] = 0x81; // binary indicator > 128
            bytes[position++] = 0x82; // binary indicator > 128
            bytes[position++] = 0x83; // binary indicator > 128
            bytes[position++] = 0x84; // binary indicator > 128
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        public uint WriteToStream(Stream stream)
        {
            var position = (uint)stream.Position;

            var bytes = new byte[ByteSize()];
            FillSpan(bytes);
            stream.Write(bytes, 0, bytes.Length);

            return position;
        }
    }
}