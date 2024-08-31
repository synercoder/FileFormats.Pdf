using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfNull : IPdfPrimitive
{
    public override string ToString()
        => "[Pdf Null]";
}
