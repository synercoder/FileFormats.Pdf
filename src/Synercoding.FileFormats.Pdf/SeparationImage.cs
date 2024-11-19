using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

namespace Synercoding.FileFormats.Pdf;

public class SeparationImage : Image
{
    internal SeparationImage(PdfReference id, Stream pixelStream, int width, int height, Separation colorSpace, SoftMask? mask)
        : base(id, pixelStream, width, height, colorSpace, mask)
    {
        Separation = colorSpace;
    }

    public Separation Separation { get; private set; }
}
