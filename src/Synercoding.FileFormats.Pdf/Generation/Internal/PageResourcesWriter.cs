using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation.Internal;

internal class PageResourcesWriter
{
    private readonly ObjectWriter _objectWriter;
    private readonly CachedResources _cachedResources;

    public PageResourcesWriter(ObjectWriter objectWriter, CachedResources cachedResources)
    {
        _objectWriter = objectWriter;
        _cachedResources = cachedResources;
    }

    public IPdfDictionary Write(PageResources pageResources)
    {
        var resourcesDict = new PdfDictionary();

        if (_writeXObjects(pageResources) is IPdfDictionary xObjectsDict)
            resourcesDict.Add(PdfNames.XObject, xObjectsDict);

        if (_writeColorSpaces(pageResources) is IPdfDictionary colorSpaceDict)
            resourcesDict.Add(PdfNames.ColorSpace, colorSpaceDict);

        if (_writeExtendedGraphicsState(pageResources) is IPdfDictionary extendedStateDict)
            resourcesDict.Add(PdfNames.ExtGState, extendedStateDict);

        if (_getFontDictionary(pageResources) is IPdfDictionary fontDictionary)
            resourcesDict.Add(PdfNames.Font, fontDictionary);

        return resourcesDict;
    }

    private IPdfDictionary? _getFontDictionary(PageResources pageResources)
    {
        if (!pageResources.FontReferences.Any())
            return null;

        var fonts = new PdfDictionary();

        foreach (var (name, id, _) in pageResources.FontReferences.Values)
            fonts.Add(name, id);

        return fonts;
    }

    private IPdfDictionary? _writeXObjects(PageResources pageResources)
    {
        if (!pageResources.Images.Any())
            return null;

        var xObjects = new PdfDictionary();
        foreach (var img in pageResources.Images)
        {
            xObjects.Add(img.Key, img.Value.Id.GetReference());
            if (!_objectWriter.TableBuiler.IsWritten(img.Value.Id))
                _objectWriter.Write(img.Value.ToStreamObject(_cachedResources));

            if (img.Value.SoftMask is { } softMask)
                if (!_objectWriter.TableBuiler.IsWritten(softMask.Id))
                    _objectWriter.Write(softMask.ToStreamObject(_cachedResources));
        }

        return xObjects;
    }

    private IPdfDictionary? _writeColorSpaces(PageResources pageResources)
    {
        if (!pageResources.SeparationReferences.Any())
            return null;

        var colorSpaces = new PdfDictionary();
        foreach (var (sep, (name, reference)) in pageResources.SeparationReferences)
        {
            colorSpaces.Add(name, reference);
            _objectWriter.Write(new PdfObject<IPdfArray>()
            {
                Id = reference.Id,
                Value = sep.ToPdfArray()
            });
        }

        return colorSpaces;
    }

    private IPdfDictionary? _writeExtendedGraphicsState(PageResources pageResources)
    {
        if (!pageResources.ExtendedGraphicsStates.Any())
            return null;

        var extendedStates = new PdfDictionary();
        foreach (var (state, (name, reference)) in pageResources.ExtendedGraphicsStates)
        {
            extendedStates.Add(name, reference);
            _objectWriter.Write(new PdfObject<IPdfDictionary>()
            {
                Id = reference.Id,
                Value = state.ToPdfDictionary()
            });
        }

        return extendedStates;
    }
}
