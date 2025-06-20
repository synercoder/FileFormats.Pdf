namespace Synercoding.FileFormats.Pdf.Primitives;

public interface IPdfStreamObject : IPdfDictionary
{
    byte[] RawData { get; }
    long Length { get; }

    IPdfDictionary AsPureDictionary();
}
