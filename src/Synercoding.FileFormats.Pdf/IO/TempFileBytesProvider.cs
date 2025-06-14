namespace Synercoding.FileFormats.Pdf.IO;

internal sealed class TempFileBytesProvider : DisposableBytesProvider
{
    private readonly string _filePath;

    public TempFileBytesProvider(Stream stream, string filePath)
        : base(stream, leaveOpen: true)
    {
        _filePath = filePath;
    }

    protected override void _dispose(bool disposing)
    {
        base._dispose(disposing);

        if(disposing)
        {
            File.Delete(_filePath);
        }
    }
}
