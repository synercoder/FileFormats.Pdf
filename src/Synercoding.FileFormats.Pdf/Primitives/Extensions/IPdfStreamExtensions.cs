using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;

public static class IPdfStreamExtensions
{
    public static byte[] DecodeData(this IPdfStreamObject streamObject, ObjectReader reader)
    {
        var decodeParams = _getDecodeParams(streamObject, reader);

        if (streamObject.TryGetValue<IPdfArray>(PdfNames.Filter, reader, out var filterArray) && filterArray.Count > 0)
        {
            var data = streamObject.RawData;
            for (int i = 0; i < filterArray.Count; i++)
            {
                if (filterArray[i] is not PdfName filterName)
                    throw new ParseException($"Filter array contained a non-PdfName value: {filterArray[i]}");

                var filter = reader.Settings.SupportedStreamFilters.Get(filterName);

                data = filter.Decode(data, i < decodeParams?.Length ? decodeParams[i] : null, reader);
            }

            return data;
        }
        else if (streamObject.TryGetValue<PdfName>(PdfNames.Filter, reader, out var filterName))
        {
            var filter = reader.Settings.SupportedStreamFilters.Get(filterName);

            var data = filter.Decode(streamObject.RawData, decodeParams?.SingleOrDefault(), reader);
            return data;
        }

        return streamObject.RawData;
    }

    private static IPdfDictionary?[]? _getDecodeParams(IPdfStreamObject streamObject, ObjectReader reader)
    {
        if (streamObject.TryGetValue<IPdfArray>(PdfNames.DecodeParms, reader, out var paramsArray) && paramsArray.Count > 0)
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
        else if (streamObject.TryGetValue<IPdfDictionary>(PdfNames.DecodeParms, reader, out var singleParameters))
        {
            return [singleParameters];
        }

        return null;
    }
}
