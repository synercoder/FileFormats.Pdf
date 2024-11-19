using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public interface IStreamFilter
{
    PdfName Name { get; }
    byte[] Encode(byte[] input, IPdfDictionary? parameters);
    byte[] Decode(byte[] input, IPdfDictionary? parameters);
}
