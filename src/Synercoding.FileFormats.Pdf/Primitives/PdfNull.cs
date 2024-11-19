using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfNull : IPdfPrimitive
{
    public static readonly PdfNull Instance = new PdfNull();

    public override string ToString()
        => "[Pdf Null]";
}
