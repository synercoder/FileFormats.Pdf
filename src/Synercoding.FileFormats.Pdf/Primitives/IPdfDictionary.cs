using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

public interface IPdfDictionary : IPdfPrimitive, IEnumerable<KeyValuePair<PdfName, IPdfPrimitive>>
{
    IPdfPrimitive? this[PdfName key] { get; }
    ICollection<PdfName> Keys { get; }
    int Count { get; }
    bool ContainsKey(PdfName key);
    bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value);
    bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive;
}
