using System;

namespace Synercoding.FileFormats.Pdf.Helpers
{
    internal interface ISpanWriteable
    {
        void FillSpan(Span<byte> bytes);
        int ByteSize();
    }
}