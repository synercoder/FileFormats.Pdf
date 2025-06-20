using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation;

public class ObjectWriter
{
    private readonly PdfStream _stream;
    private readonly long _pdfStart;

    public ObjectWriter(PdfStream stream, long pdfStart)
        : this(new TableBuilder(), stream, pdfStart)
    { }

    internal ObjectWriter(TableBuilder tableBuilder, PdfStream stream, long pdfStart)
    {
        TableBuiler = tableBuilder;
        _stream = stream;
        _pdfStart = pdfStart;
    }

    internal TableBuilder TableBuiler { get; }

    public ObjectWriter Write<T>(PdfObject<T> pdfObject)
        where T : IPdfPrimitive
    {
        if (pdfObject is PdfObject<IPdfStreamObject> streamObj)
            return Write(streamObj);

        var relativePosition = _stream.Position - _pdfStart;

        if (!TableBuiler.TrySetPosition(pdfObject.Id, relativePosition))
            throw new ArgumentException("Pdf object is already written, or belongs to a different writer.", nameof(pdfObject));

        _stream
            .Write(pdfObject.Id.ObjectNumber)
            .Space()
            .Write(pdfObject.Id.Generation)
            .Space()
            .Write("obj")
            .NewLine()
            .WriteDirect(pdfObject.Value)
            .NewLine()
            .Write("endobj")
            .NewLine();

        return this;
    }

    public ObjectWriter Write(PdfObject<IPdfStreamObject> pdfStreamObject)
    {
        var relativePosition = _stream.Position - _pdfStart;

        if (!TableBuiler.TrySetPosition(pdfStreamObject.Id, relativePosition))
            throw new ArgumentException("Pdf object is already written, or belongs to a different writer.", nameof(pdfStreamObject));

        if (!pdfStreamObject.Value.ContainsKey(PdfNames.Length))
            throw new ArgumentException("The stream object is invalid since it doesn't contain a /Length key.", nameof(pdfStreamObject));

        _stream
            .Write(pdfStreamObject.Id.ObjectNumber)
            .Space()
            .Write(pdfStreamObject.Id.Generation)
            .Space()
            .Write("obj")
            .NewLine()
            .WriteDirect(pdfStreamObject.Value.AsPureDictionary())
            .NewLine()
            .Write("stream")
            .NewLine()
            .Write(pdfStreamObject.Value.RawData)
            .Write("endstream")
            .NewLine()
            .Write("endobj")
            .NewLine();

        return this;
    }
}
