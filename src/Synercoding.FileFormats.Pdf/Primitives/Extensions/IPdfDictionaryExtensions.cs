using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Logging;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;

public static class IPdfDictionaryExtensions
{
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

    internal static void ValidateDictionaryType<TLoggerCategory>(this IPdfDictionary dictionary, PdfObjectId id, ObjectReader objectReader, PdfName expectedType)
    {
        var logger = objectReader.Settings.Logger;

        if (!dictionary.TryGetValue<PdfName>(PdfNames.Type, objectReader, out var actualType))
        {
            logger.LogWarning<TLoggerCategory>("The dictionary (id {Id}) does not contain the required key {MissingKey}",
                id, PdfNames.Type);
            if (objectReader.Settings.Strict)
                throw new ParseException($"The dictionary (id {id}) does not contain the required key {PdfNames.Type}");
        }
        else if (actualType != expectedType)
        {
            logger.LogWarning<TLoggerCategory>("The dictionary (id {Id}) Type key did not contain a value of {ExpectedValue}, instead it had: {FoundValue}",
                id, expectedType, actualType);
            if (objectReader.Settings.Strict)
                throw new ParseException($"The dictionary (id {id}) Type key did not contain a value of {expectedType}, instead it had: {actualType}");
        }
    }
}
