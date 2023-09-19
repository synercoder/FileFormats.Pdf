using System.Text;

namespace Synercoding.FileFormats.Pdf.LowLevel;

/// <summary>
/// A class representing a name object in a pdf.
/// </summary>
public class PdfName
{
    private static readonly IDictionary<string, PdfName> _reservedNames = new Dictionary<string, PdfName>()
    {
        { "Author", new PdfName("Author") },
        { "BitsPerComponent", new PdfName("BitsPerComponent") },
        { "BleedBox", new PdfName("BleedBox") },
        { "Catalog", new PdfName("Catalog") },
        { "ColorSpace", new PdfName("ColorSpace") },
        { "Contents", new PdfName("Contents") },
        { "Count", new PdfName("Count") },
        { "CreationDate", new PdfName("CreationDate") },
        { "CropBox", new PdfName("CropBox") },
        { "DCTDecode", new PdfName("DCTDecode") },
        { "Decode", new PdfName("Decode") },
        { "DeviceCMYK", new PdfName("DeviceCMYK") },
        { "DeviceGray", new PdfName("DeviceGray") },
        { "DeviceRGB", new PdfName("DeviceRGB") },
        { "Filter", new PdfName("Filter") },
        { "Height", new PdfName("Height") },
        { "Image", new PdfName("Image") },
        { "Info", new PdfName("Info") },
        { "Kids", new PdfName("Kids") },
        { "Length", new PdfName("Length") },
        { "MediaBox", new PdfName("MediaBox") },
        { "Page", new PdfName("Page") },
        { "Pages", new PdfName("Pages") },
        { "Parent", new PdfName("Parent") },
        { "Producer", new PdfName("Producer") },
        { "Resources", new PdfName("Resources") },
        { "Root", new PdfName("Root") },
        { "Rotate", new PdfName("Rotate") },
        { "Size", new PdfName("Size") },
        { "Subtype", new PdfName("Subtype") },
        { "Title", new PdfName("Title") },
        { "TrimBox", new PdfName("TrimBox") },
        { "Type", new PdfName("Type") },
        { "Width", new PdfName("Width") },
        { "XObject", new PdfName("XObject") },
    };

    private readonly string _encoded;

    private PdfName(string raw)
    {
        if (raw.Length > 127)
            throw new ArgumentOutOfRangeException(nameof(raw), "The name is too long. Max length of 127 is allowed.");
        if (raw.Any(c => c > 0xff))
            throw new ArgumentOutOfRangeException(nameof(raw), "The name contains non-ascii characters, and is thus not allowed.");
        _encoded = _encode(raw);
    }

    /// <inheritdoc />
    public override int GetHashCode()
        => HashCode.Combine(_encoded, "PdfName");

    /// <inheritdoc />
    public override string ToString()
        => _encoded;

    /// <summary>
    /// Get a <see cref="PdfName"/> from a given <see cref="string"/>
    /// </summary>
    /// <param name="name">The name in raw string form</param>
    /// <returns>A <see cref="PdfName"/> that is based on the given <paramref name="name"/>.</returns>
    public static PdfName Get(string name)
    {
        if (_reservedNames.TryGetValue(name, out var value))
            return value;

        System.Diagnostics.Debug.WriteLine($"{{ \"{name}\", new PdfName(\"{name}\") }},");

        return new PdfName(name);
    }

    private static string _encode(string input)
    {
        int length = input.Length;

        var pdfName = new StringBuilder(length + 20);
        pdfName.Append('/');
        var chars = input.ToCharArray();

        foreach (var c in chars)
        {
            // 0xff because only ascii is supported in pdf names
            var encoded = c switch
            {
                (char)0 => throw new InvalidOperationException("NULL character is not allowed in a pdf name."),
                var cc when cc >= 'a' && cc <= 'z' => cc.ToString(),
                var cc when cc >= 'A' && cc <= 'Z' => cc.ToString(),
                var cc when cc >= '0' && cc <= '9' => cc.ToString(),
                // special characters
                var cc when cc < 16 => $"#0{Convert.ToString(cc, 16)}",
                var cc => '#' + Convert.ToString(cc, 16),
            };
            pdfName.Append(encoded);
        }

        return pdfName.ToString();
    }
}
