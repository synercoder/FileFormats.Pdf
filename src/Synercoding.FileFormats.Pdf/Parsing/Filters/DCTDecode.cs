using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class DCTDecode : IStreamFilter
{
    public PdfName Name => PdfNames.DCTDecode;

    public byte[] Decode(byte[] input, IPdfDictionary? parameters, ObjectReader objectReader)
    {
        return input;
    }

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        throw new NotImplementedException();
    }
}
