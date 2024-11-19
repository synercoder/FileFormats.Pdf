using Synercoding.FileFormats.Pdf.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;
public static class PrimitiveExtensions
{
    public static bool TryGetValue<TPrimitive>(this PdfArray pdfArray, int index, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
    {
        if (index >= pdfArray.Count)
            throw new IndexOutOfRangeException();

        if (pdfArray[index] is TPrimitive primitive)
        {
            value = primitive;
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetValue<TPrimitive>(this PdfArray pdfArray, int index, ObjectReader reader, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
    {
        value = default;

        if (index >= pdfArray.Count)
            throw new IndexOutOfRangeException();

        if (pdfArray[index] is TPrimitive primitive)
        {
            value = primitive;
            return true;
        }

        return pdfArray[index] is PdfReference reference
            && reader.TryGet(reference.Id, out value);
    }

    public static bool TryGetValue<TPrimitive>(this IPdfDictionary dictionary, PdfName key, ObjectReader reader, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
    {
        value = default;

        if (!dictionary.TryGetValue(key, out IPdfPrimitive? primitive))
            return false;

        if (primitive is TPrimitive correctType)
        {
            value = correctType;
            return true;
        }

        return value is PdfReference reference
            && reader.TryGet(reference.Id, out value);
    }

    public static byte[] DecodeData(this IPdfStreamObject streamObject, ObjectReader reader)
    {
        var decodeParams = _getDecodeParams(streamObject, reader);

        if (streamObject.TryGetValue<PdfArray>(PdfNames.Filter, reader, out var filterArray) && filterArray.Count > 0)
        {
            var data = streamObject.RawData;
            for (int i = 0; i < filterArray.Count; i++)
            {
                if (filterArray[i] is not PdfName filterName)
                    throw new ParseException($"Filter array contained a non-PdfName value: {filterArray[i]}");

                var filter = reader.Settings.SupportedStreamFilters.Get(filterName);

                data = filter.Decode(data, i < decodeParams?.Length ? decodeParams[i] : null);
            }

            return data;
        }
        else if (streamObject.TryGetValue<PdfName>(PdfNames.Filter, reader, out var filterName))
        {
            var filter = reader.Settings.SupportedStreamFilters.Get(filterName);

            var data = filter.Decode(streamObject.RawData, decodeParams?.SingleOrDefault());
            return data;
        }

        return streamObject.RawData;
    }

    private static IPdfDictionary?[]? _getDecodeParams(IPdfStreamObject streamObject, ObjectReader reader)
    {
        if (streamObject.TryGetValue<PdfArray>(PdfNames.DecodeParms, reader, out var paramsArray) && paramsArray.Count > 0)
        {
            var dictionaries = new IPdfDictionary?[paramsArray.Count];
            for (int i = 0; i < paramsArray.Count; i++)
            {
                if (paramsArray[i] is IPdfDictionary decodeParameter)
                    dictionaries[i] = decodeParameter;
                else
                    dictionaries[i] = null;
            }
            return dictionaries;
        }
        else if (streamObject.TryGetValue<PdfDictionary>(PdfNames.DecodeParms, reader, out var singleParameters))
        {
            return [singleParameters];
        }

        return null;
    }
}
