using Synercoding.FileFormats.Pdf.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Represents a PDF version with major and minor version numbers.
/// </summary>
/// <param name="Major">The major version number.</param>
/// <param name="Minor">The minor version number.</param>
public sealed record PdfVersion(byte Major, byte Minor)
{
    /// <summary>
    /// Attempts to parse a PDF version from a PdfName.
    /// </summary>
    /// <param name="nameVersion">The PdfName containing the version string (e.g., "1.7").</param>
    /// <param name="version">When this method returns, contains the parsed PdfVersion if successful; otherwise, null.</param>
    /// <returns>True if parsing was successful; otherwise, false.</returns>
    public static bool TryParse(PdfName nameVersion, [NotNullWhen(true)] out PdfVersion? version)
    {
        version = null;

        var dotIndex = nameVersion.Display.IndexOf('.');
        if (dotIndex == -1)
            return false;

        if (!int.TryParse(nameVersion.Display[..dotIndex], out int major) || major > 0xFF || major < 0)
            return false;

        if (!int.TryParse(nameVersion.Display[( dotIndex + 1 )..], out int minor) || minor > 0xFF || minor < 0)
            return false;

        version = new PdfVersion((byte)major, (byte)minor);

        return true;
    }
}
