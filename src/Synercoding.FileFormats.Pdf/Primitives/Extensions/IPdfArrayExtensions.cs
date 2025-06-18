using Synercoding.FileFormats.Pdf.Parsing;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives.Extensions;

public static class IPdfArrayExtensions
{
    public static bool TryGetValue<TPrimitive>(this IPdfArray pdfArray, int index, [NotNullWhen(true)] out TPrimitive? value)
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

    public static bool TryGetValue<TPrimitive>(this IPdfArray pdfArray, int index, ObjectReader reader, [NotNullWhen(true)] out TPrimitive? value)
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

    public static bool TryGetAsRectangle(this IPdfArray array, ObjectReader objectReader, [NotNullWhen(true)] out Rectangle? rectangle)
        => TryGetAsRectangle(array, objectReader, null, out rectangle);

    public static bool TryGetAsRectangle(this IPdfArray array, ObjectReader objectReader, double? userUnits, [NotNullWhen(true)] out Rectangle? rectangle)
    {
        _ = array ?? throw new ArgumentNullException(nameof(array));

        rectangle = null;

        if (array.Count != 4)
            return false;

        if (!array.TryGetValue<PdfNumber>(0, objectReader, out var llx))
            return false;

        if (!array.TryGetValue<PdfNumber>(1, objectReader, out var lly))
            return false;

        if (!array.TryGetValue<PdfNumber>(2, objectReader, out var urx))
            return false;

        if (!array.TryGetValue<PdfNumber>(3, objectReader, out var ury))
            return false;

        var unit = userUnits.HasValue && userUnits.Value != 1
            ? Unit.Pixels(userUnits.Value / 72)
            : Unit.Points;

        rectangle = new Rectangle(llx, lly, urx, ury, unit);

        return true;
    }
}
