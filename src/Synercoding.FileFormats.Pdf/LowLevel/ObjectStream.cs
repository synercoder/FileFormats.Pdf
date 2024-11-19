using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Internal;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.FileFormats.Pdf.LowLevel.XRef;
using System.IO.Compression;

namespace Synercoding.FileFormats.Pdf.LowLevel;

internal class ObjectStream
{
    private const byte BRACKET_OPEN = 0x5B;  // [
    private const byte BRACKET_CLOSE = 0x5D; // ]

    private readonly TableBuilder _tableBuilder;

    public ObjectStream(PdfStream stream, TableBuilder tableBuilder)
    {
        InnerStream = stream;
        _tableBuilder = tableBuilder;
    }

    public PdfStream InnerStream { get; }

    public ObjectStream Write(ContentStream contentStream)
    {
        if (!_tableBuilder.TrySetPosition(contentStream.Reference, InnerStream.Position))
            return this;

        using (var flateStream = PdfWriter.FlateEncode(contentStream.InnerStream.InnerStream))
            _indirectStream(contentStream.Reference, flateStream, StreamFilter.FlateDecode);

        return this;
    }

    public ObjectStream Write(Catalog catalog)
    {
        if (!_tableBuilder.TrySetPosition(catalog.Reference, InnerStream.Position))
            return this;

        _indirectDictionary(catalog.Reference, catalog, static (catalog, dictionary) =>
        {
            dictionary
                .Write(PdfName.Get("Type"), PdfName.Get("Catalog"))
                .Write(PdfName.Get("Pages"), catalog.PageTree.Reference);
        });

        return this;
    }

    public ObjectStream Write(DocumentInformation documentInformation)
    {
        if (!_tableBuilder.TrySetPosition(documentInformation.Reference, InnerStream.Position))
            return this;

        _indirectDictionary(documentInformation.Reference, documentInformation, static (did, dictionary) =>
        {
            dictionary
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Title"), did.Title)
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Author"), did.Author)
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Subject"), did.Subject)
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Keywords"), did.Keywords)
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Creator"), did.Creator)
                .WriteHexadecimalIfNotNullOrWhiteSpace(PdfName.Get("Producer"), did.Producer)
                .WriteIfNotNull(PdfName.Get("CreationDate"), did.CreationDate)
                .WriteIfNotNull(PdfName.Get("ModDate"), did.ModDate);

            if (did.ExtraInfo.Count != 0)
                foreach (var kv in did.ExtraInfo)
                    dictionary.WriteHexadecimalString(PdfName.Get(kv.Key), kv.Value);
        });

        return this;
    }

    public ObjectStream Write(PageTree pageTree)
    {
        if (!_tableBuilder.TrySetPosition(pageTree.Reference, InnerStream.Position))
            return this;

        _indirectDictionary(pageTree.Reference, pageTree, static (pageTree, dictionary) =>
        {
            dictionary
                .Write(PdfName.Get("Type"), PdfName.Get("Pages"))
                .Write(PdfName.Get("Kids"), pageTree.Pages.Select(static p => p.Reference).ToArray())
                .Write(PdfName.Get("Count"), pageTree.Pages.Count);
        });

        return this;
    }

    public ObjectStream Write(Image image)
    {
        if (!_tableBuilder.TrySetPosition(image.Reference, InnerStream.Position))
            return this;

        _indirectStream(image.Reference, image.RawStream, (image, _tableBuilder), static (tuple, dictionary) =>
        {
            var (image, tableBuilder) = tuple;
            dictionary
                .Write(PdfName.Get("Type"), PdfName.Get("XObject"))
                .Write(PdfName.Get("Subtype"), PdfName.Get("Image"))
                .Write(PdfName.Get("Width"), image.Width)
                .Write(PdfName.Get("Height"), image.Height)
                .Write(PdfName.Get("BitsPerComponent"), 8)
                .Write(PdfName.Get("Decode"), _decodeArray(image.ColorSpace))
                .WriteIfNotNull(PdfName.Get("SMask"), image.SoftMask?.Reference);


            if (image.ColorSpace is Separation separation)
            {
                var sepId = tableBuilder.GetSeparationId(separation);
                dictionary.Write(PdfName.Get("ColorSpace"), sepId);
            }
            else
            {
                dictionary.Write(PdfName.Get("ColorSpace"), image.ColorSpace.Name);
            }
        }, image.Filters);

        if (image.SoftMask != null)
            Write(image.SoftMask);

        if(image.ColorSpace is Separation separation)
            Write(separation);

        return this;

        static double[] _decodeArray(ColorSpace colorSpace)
            => Enumerable.Range(0, colorSpace.Components)
                .Select(_ => new double[] { 0, 1 })
                .SelectMany(x => x)
                .ToArray();
    }

    public ObjectStream Write(PdfPage page)
    {
        if (!_tableBuilder.TrySetPosition(page.Reference, InnerStream.Position))
            return this;

        _indirectDictionary(page.Reference, page, static (page, dictionary) =>
        {
            dictionary
                .Write(PdfName.Get("Type"), PdfName.Get("Page"))
                .Write(PdfName.Get("Parent"), page.Parent)
                .Write(PdfName.Get("MediaBox"), page.MediaBox)
                .WriteIfNotNull(PdfName.Get("CropBox"), page.CropBox)
                .WriteIfNotNull(PdfName.Get("BleedBox"), page.BleedBox)
                .WriteIfNotNull(PdfName.Get("TrimBox"), page.TrimBox)
                .WriteIfNotNull(PdfName.Get("ArtBox"), page.ArtBox)
                .WriteIfNotNull(PdfName.Get("Rotate"), (int?)page.Rotation);

            // Resources
            dictionary.Write(PdfName.Get("Resources"), page.Resources, static (resources, stream) => stream.Dictionary(resources, static (resources, stream) =>
            {
                if (resources.Images.Count != 0)
                {
                    stream.Write(PdfName.Get("XObject"), resources.Images, static (images, stream) => stream.Dictionary(images, static (images, xobject) =>
                    {
                        foreach (var image in images)
                        {
                            xobject.Write(image.Key, image.Value.Reference);
                        }
                    }));
                }

                if (resources.FontReferences.Count != 0)
                {
                    stream.Write(PdfName.Get("Font"), resources.FontReferences, static (fonts, stream) => stream.Dictionary(fonts, static (fontReferences, fontDictionary) =>
                    {
                        foreach (var (font, reference) in fontReferences)
                        {
                            fontDictionary.Write(font.LookupName, reference);
                        }
                    }));
                }

                if (resources.SeparationReferences.Count != 0)
                {
                    stream.Write(PdfName.Get("ColorSpace"), resources.SeparationReferences.Values, static (separations, stream) => stream.Dictionary(separations, static (separations, colorspaceDictionary) =>
                    {
                        foreach (var (name, reference) in separations)
                        {
                            colorspaceDictionary.Write(name, reference);
                        }
                    }));
                }

                if (resources.ExtendedGraphicsStates.Count != 0)
                {
                    stream.Write(PdfName.Get("ExtGState"), resources.ExtendedGraphicsStates.Values, static (extGStates, stream) => stream.Dictionary(extGStates, static (extendedGStates, dict) =>
                    {
                        foreach (var (name, reference) in extendedGStates)
                        {
                            dict.Write(name, reference);
                        }
                    }));
                }

            }));

            // Content stream
            dictionary.Write(PdfName.Get("Contents"), page.Content.RawContentStream.Reference);
        });

        return this;
    }

    public ObjectStream Write(PdfReference reference, Type1StandardFont font)
    {
        if (!_tableBuilder.TrySetPosition(reference, InnerStream.Position))
            return this;

        _indirectDictionary(reference, font, static (font, dict) =>
        {
            dict
                .Write(PdfName.Get("Type"), PdfName.Get("Font"))
                .Write(PdfName.Get("Subtype"), PdfName.Get("Type1"))
                .Write(PdfName.Get("BaseFont"), font.Name);
        });

        return this;
    }

    public ObjectStream Write(Separation separation)
    {
        var id = _tableBuilder.GetSeparationId(separation);

        if (!_tableBuilder.TrySetPosition(id, InnerStream.Position))
            return this;

        InnerStream
            .StartObject(id)
            .WriteByte(BRACKET_OPEN)
            .Write(PdfName.Get("Separation"))
            .Write(separation.Name)
            .Write(separation.BasedOnColor.Colorspace.Name)
            .Dictionary(separation.BasedOnColor, static (color, dict) =>
            {
                var c0 = new double[color.Colorspace.Components];
                var c1 = color.Components;
                var range = new double[color.Colorspace.Components * 2];
                for (int i = 1; i < range.Length; i += 2)
                    range[i] = 1;

                dict.Write(PdfName.Get("C0"), c0)
                    .Write(PdfName.Get("C1"), c1)
                    .Write(PdfName.Get("Domain"), 0, 1)
                    .Write(PdfName.Get("FunctionType"), 2)
                    .Write(PdfName.Get("N"), 1.0)
                    .Write(PdfName.Get("Range"), range);
            })
            .WriteByte(BRACKET_CLOSE)
            .EndObject()
            .NewLine();

        return this;
    }

    public ObjectStream Write(PdfReference reference, ExtendedGraphicsState state)
    {
        if (!_tableBuilder.TrySetPosition(reference, InnerStream.Position))
            return this;

        _indirectDictionary(reference, state, static (state, dict) =>
        {
            if (state.Overprint.HasValue)
                dict.Write(PdfName.Get("OP"), state.Overprint.Value);
            if (state.OverprintNonStroking.HasValue)
                dict.Write(PdfName.Get("op"), state.OverprintNonStroking.Value);
        });

        return this;
    }

    private void _indirectDictionary<T>(PdfReference reference, T data, Action<T, PdfDictionary> dictionaryAction)
    {
        InnerStream
            .StartObject(reference)
            .Dictionary(data, dictionaryAction)
            .EndObject()
            .NewLine();
    }

    private void _indirectStream(PdfReference reference, Stream stream, params StreamFilter[] streamFilters)
        => _indirectStream(reference, stream, _ => { }, streamFilters);

    private void _indirectStream(PdfReference reference, Stream stream, Action<PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
        => _indirectStream(reference, stream, dictionaryAction, static (action, dict) => action(dict), streamFilters);

    private void _indirectStream<TData>(PdfReference reference, Stream stream, TData data, Action<TData, PdfDictionary> dictionaryAction, params StreamFilter[] streamFilters)
            where TData : notnull
    {
        InnerStream
            .StartObject(reference)
            .Dictionary((stream.Length, data, dictionaryAction, streamFilters), static (tuple, dictionary) =>
            {
                var (length, data, dictionaryAction, streamFilters) = tuple;
                dictionary.Write(PdfName.Get("Length"), length);
                if (streamFilters != null && tuple.streamFilters.Length > 0)
                {
                    if (streamFilters.Length == 1)
                    {
                        dictionary.Write(PdfName.Get("Filter"), streamFilters[0].ToPdfName());
                    }
                    else
                    {
                        dictionary.Write(PdfName.Get("Filter"), streamFilters.Select(f => f.ToPdfName()).ToArray());
                    }
                }

                dictionaryAction(data, dictionary);
            })
            .Write("stream")
            .NewLine()
            .CopyFrom(stream)
            .NewLine()
            .Write("endstream")
            .NewLine()
            .EndObject()
            .NewLine();
    }
}
