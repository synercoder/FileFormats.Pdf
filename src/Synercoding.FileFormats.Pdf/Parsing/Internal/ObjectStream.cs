using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Parsing.Internal;

internal class ObjectStream
{
    private readonly Parser _parser;
    private readonly IDictionary<PdfObjectId, long> _offsetLookup = new Dictionary<PdfObjectId, long>();

    public ObjectStream(IPdfStreamObject streamDictionary, ObjectReader reader)
    {
        _ = streamDictionary ?? throw new ArgumentNullException(nameof(streamDictionary));
        if (!streamDictionary.TryGetValue<PdfName>(PdfNames.Type, reader, out var type) || type != PdfNames.ObjStm)
            throw new ArgumentException("Provided stream object is not of the correct type.", nameof(streamDictionary));

        if (!streamDictionary.TryGetValue<PdfInteger>(PdfNames.N, reader, out var numberOfObjects))
            throw new ArgumentException("Provided stream object does not have the N parameter.");

        if (!streamDictionary.TryGetValue<PdfInteger>(PdfNames.First, reader, out var firstOffSet))
            throw new ArgumentException("Provided stream object does not have the First parameter.");

        var bytesProvider = new PdfByteArrayProvider(streamDictionary.DecodeData(reader));
        var tokenizer = new Tokenizer(bytesProvider, reader.Settings.Logger);
        _parser = new Parser(tokenizer, reader.Settings.Logger);

        _readOffsets(firstOffSet.Value, numberOfObjects.Value);
    }

    public CompressedXRefItem[] AsXRefItems(PdfObjectId wrapperStreamId)
        => _offsetLookup
            .OrderBy(kv => kv.Value)
            .Select((kv, i) => new CompressedXRefItem(kv.Key, wrapperStreamId, i))
            .ToArray();

    public bool TryGet<TPrimitive>(PdfObjectId id, [NotNullWhen(true)] out TPrimitive? pdfObject)
        where TPrimitive : IPdfPrimitive
    {
        pdfObject = default;

        if (!_offsetLookup.TryGetValue(id, out var offset))
            return false;

        _parser.Tokenizer.Position = offset;

        var objectAtOffset = _parser.ReadNext();

        if (objectAtOffset is not TPrimitive correctType)
            return false;

        pdfObject = correctType;

        return true;
    }

    private void _readOffsets(long firstOffset, long numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            var objectId = _parser.ReadInteger();
            _parser.Tokenizer.PdfBytesProvider.SkipWhiteSpace();
            var offset = _parser.ReadInteger() + firstOffset;

            _offsetLookup[new PdfObjectId((int)objectId, 0)] = offset;
        }
    }
}
