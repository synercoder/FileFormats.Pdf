using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors.ColorSpaces;

namespace Synercoding.FileFormats.Pdf;

public class SoftMask : Image
{
    internal SoftMask(PdfReference id, Stream pixelStream, int width, int height)
        : base(id, pixelStream, width, height, DeviceGray.Instance, null)
    { }
}
