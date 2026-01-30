using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents the PDF null primitive value.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct PdfNull : IPdfPrimitive
{
    /// <summary>
    /// Gets the singleton instance of <see cref="PdfNull"/>.
    /// </summary>
    public static readonly PdfNull INSTANCE = new PdfNull();

    /// <inheritdoc />
    public override string ToString()
        => "[Pdf Null]";
}
