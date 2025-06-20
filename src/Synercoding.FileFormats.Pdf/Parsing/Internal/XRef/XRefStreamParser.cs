using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal.XRef;

internal class XRefStreamParser : IXRefParser
{
    public bool CanParse(IPdfBytesProvider pdfBytesProvider, long xrefPosition)
    {
        var originalPosition = pdfBytesProvider.Position;
        try
        {
            pdfBytesProvider.Seek(xrefPosition, SeekOrigin.Begin);
                        
            // XRef streams start with an object number, generation, "obj" keyword
            // We'll look for a pattern like "nnn nnn obj" to identify XRef streams
            if (!pdfBytesProvider.TryRead(20, out var bytes)) // Read enough bytes to check pattern
                return false;
                
            var content = System.Text.Encoding.ASCII.GetString(bytes);
            return content.Contains(" obj") && !content.StartsWith("xref");
        }
        catch
        {
            return false;
        }
        finally
        {
            pdfBytesProvider.Seek(originalPosition, SeekOrigin.Begin);
        }
    }

    public (Trailer Trailer, XRefTable XRefTable) Parse(IPdfBytesProvider pdfBytesProvider, long pdfStart, long xrefPosition, ObjectReader reader)
    {
        pdfBytesProvider.Seek(pdfStart + xrefPosition, SeekOrigin.Begin);

        var lexer = new Lexer(pdfBytesProvider, reader.Settings.Logger);
        var parser = new Parser(lexer, reader.Settings.Logger);
        var streamObjectWrap = parser.ReadObject<IPdfStreamObject>();
        var streamObject = streamObjectWrap.Value;

        var xRefItems = XRefStream.ParseStream(streamObject, reader);
        var trailer = new Trailer(streamObject, reader.Settings);
        var table = new XRefTable(xRefItems);

        // Sometimes the xrefstream object itself is not referenced inside an xref table. Ensure it is by always setting it
        table.SetItem(new ClassicXRefItem(streamObjectWrap.Id, xrefPosition));

        return (trailer, table);
    }
}
