namespace Synercoding.FileFormats.Pdf.Primitives;

public interface IPdfStream : IPdfDictionary
{
    byte[] RawData { get; }
    long Length { get; }
}
