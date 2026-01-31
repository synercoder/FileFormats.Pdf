using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation;

/// <summary>
/// Writes PDF objects to a PDF stream with automatic position tracking and cross-reference table building.
/// </summary>
public class ObjectWriter
{
    private readonly PdfStream _stream;
    private readonly DirectObjectSerializer _serializer;
    private readonly long _pdfStart;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectWriter"/> class.
    /// </summary>
    /// <param name="stream">The PDF stream to write objects to.</param>
    /// <param name="pdfStart">The byte position where the PDF content starts in the stream.</param>
    public ObjectWriter(PdfStream stream, long pdfStart)
        : this(new TableBuilder(), stream, pdfStart)
    { }

    internal ObjectWriter(TableBuilder tableBuilder, PdfStream stream, long pdfStart)
    {
        TableBuiler = tableBuilder;
        _stream = stream;
        _serializer = new DirectObjectSerializer(stream);
        _pdfStart = pdfStart;
    }

    internal TableBuilder TableBuiler { get; }

    /// <summary>
    /// Writes a PDF object to the stream and records its position in the cross-reference table.
    /// </summary>
    /// <typeparam name="T">The type of the PDF primitive contained in the object.</typeparam>
    /// <param name="pdfObject">The PDF object to write.</param>
    /// <returns>This <see cref="ObjectWriter"/> instance for method chaining.</returns>
    public ObjectWriter Write<T>(PdfObject<T> pdfObject)
        where T : IPdfPrimitive
    {
        if (pdfObject is PdfObject<IPdfStreamObject> streamObjWithId)
            return Write(streamObjWithId);

        if (pdfObject.Value is IPdfStreamObject streamObj)
        {
            return Write(new PdfObject<IPdfStreamObject>()
            {
                Id = pdfObject.Id,
                Value = streamObj
            });
        }

        var relativePosition = _stream.Position - _pdfStart;

        if (!TableBuiler.TrySetPosition(pdfObject.Id, relativePosition))
            return this; // Object already written

        _stream
            .Write(pdfObject.Id.ObjectNumber)
            .Space()
            .Write(pdfObject.Id.Generation)
            .Space()
            .Write("obj")
            .NewLine();
        _serializer.WriteDirect(pdfObject.Value);
        _stream
            .NewLine()
            .Write("endobj")
            .NewLine();

        return this;
    }

    /// <summary>
    /// Writes a PDF stream object to the stream with its dictionary and stream data.
    /// </summary>
    /// <param name="pdfStreamObject">The PDF stream object to write.</param>
    /// <returns>This <see cref="ObjectWriter"/> instance for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the stream object doesn't contain a required /Length key.</exception>
    public ObjectWriter Write(PdfObject<IPdfStreamObject> pdfStreamObject)
    {
        var relativePosition = _stream.Position - _pdfStart;

        if (!TableBuiler.TrySetPosition(pdfStreamObject.Id, relativePosition))
            return this; // Object already written

        if (!pdfStreamObject.Value.ContainsKey(PdfNames.Length))
            throw new ArgumentException("The stream object is invalid since it doesn't contain a /Length key.", nameof(pdfStreamObject));

        _stream
            .Write(pdfStreamObject.Id.ObjectNumber)
            .Space()
            .Write(pdfStreamObject.Id.Generation)
            .Space()
            .Write("obj")
            .NewLine();
        _serializer.WriteDirect(pdfStreamObject.Value.AsPureDictionary());
        _stream
            .NewLine()
            .Write("stream")
            .NewLine()
            .Write(pdfStreamObject.Value.RawData)
            .NewLine()
            .Write("endstream")
            .NewLine()
            .Write("endobj")
            .NewLine();

        return this;
    }
}
