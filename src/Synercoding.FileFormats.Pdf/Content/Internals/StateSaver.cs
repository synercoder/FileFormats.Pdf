namespace Synercoding.FileFormats.Pdf.Content.Internals;

internal class StateSaver<TContentContext> : IDisposable
    where TContentContext : IContentContext<TContentContext>
{
    private readonly TContentContext _context;

    public StateSaver(TContentContext context)
    {
        _context = context;

        _context.RawContentStream.SaveState();
    }

    public void Dispose()
    {
        _context.RawContentStream.RestoreState();
    }
}
