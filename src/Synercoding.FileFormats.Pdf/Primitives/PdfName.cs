using Synercoding.FileFormats.Pdf.IO;
using System.Diagnostics;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public sealed class PdfName : IPdfPrimitive, IEquatable<PdfName>
{
    private static readonly IDictionary<string, PdfName> _reservedNames = new Dictionary<string, PdfName>()
    {
        { "Author",           new PdfName("Author") },
        { "BitsPerComponent", new PdfName("BitsPerComponent") },
        { "BleedBox",         new PdfName("BleedBox") },
        { "Catalog",          new PdfName("Catalog") },
        { "ColorSpace",       new PdfName("ColorSpace") },
        { "Contents",         new PdfName("Contents") },
        { "Count",            new PdfName("Count") },
        { "CreationDate",     new PdfName("CreationDate") },
        { "CropBox",          new PdfName("CropBox") },
        { "DCTDecode",        new PdfName("DCTDecode") },
        { "Decode",           new PdfName("Decode") },
        { "DeviceCMYK",       new PdfName("DeviceCMYK") },
        { "DeviceGray",       new PdfName("DeviceGray") },
        { "DeviceRGB",        new PdfName("DeviceRGB") },
        { "Filter",           new PdfName("Filter") },
        { "Height",           new PdfName("Height") },
        { "Image",            new PdfName("Image") },
        { "Info",             new PdfName("Info") },
        { "Kids",             new PdfName("Kids") },
        { "Length",           new PdfName("Length") },
        { "MediaBox",         new PdfName("MediaBox") },
        { "Page",             new PdfName("Page") },
        { "Pages",            new PdfName("Pages") },
        { "Parent",           new PdfName("Parent") },
        { "Producer",         new PdfName("Producer") },
        { "Resources",        new PdfName("Resources") },
        { "Root",             new PdfName("Root") },
        { "Rotate",           new PdfName("Rotate") },
        { "Size",             new PdfName("Size") },
        { "Subtype",          new PdfName("Subtype") },
        { "Title",            new PdfName("Title") },
        { "TrimBox",          new PdfName("TrimBox") },
        { "Type",             new PdfName("Type") },
        { "Width",            new PdfName("Width") },
        { "XObject",          new PdfName("XObject") },
    };

    /// <summary>
    /// Get a <see cref="PdfName"/> from a given <see cref="string"/>
    /// </summary>
    /// <param name="name">The name in raw string form</param>
    /// <returns>A <see cref="PdfName"/> that is based on the given <paramref name="name"/>.</returns>
    public static PdfName Get(string name)
    {
        if (_reservedNames.TryGetValue(name, out var value))
            return value;

#if DEBUG
        Debug.WriteLine($"{{ \"{name}\", new PdfName(\"{name}\") }},");
        Debug.WriteLine($"public static PdfName {name} => PdfName.Get(\"{name}\");");
#endif

        return new PdfName(name);
    }

    internal PdfName(byte[] bytes)
    {
        Raw = bytes ?? throw new ArgumentNullException(nameof(bytes));
        Display = Encoding.UTF8.GetString(_unescape(bytes));
    }

    private PdfName(string input)
    {
        Raw = _escape(Encoding.UTF8.GetBytes(input));
        Display = input;
    }

    internal byte[] Raw { get; }
    public string Display { get; }

    public bool Equals(PdfName? other)
        => other is not null && Raw.SequenceEqual(other.Raw);

    public override bool Equals(object? obj)
        => obj is PdfName pdfName && Equals(pdfName);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(Raw);
        return hashCode.ToHashCode();
    }

    [DebuggerStepThrough]
    public override string ToString()
        => $"[Pdf Name] /{Encoding.UTF8.GetString(Raw)}";

    private static byte[] _unescape(byte[] input)
    {
        var output = new List<byte>(input.Length);

        for (int index = 0; index < input.Length; index++)
        {
            if (input[index] == 0x23)
            {
                output.Add(FromHex(input[index + 1], input[index + 2]));
                index += 2;
            }
            else
            {
                output.Add(input[index]);
            }
        }

        return output.ToArray();

        static byte FromHex(byte b1, byte b2)
        {
            return (byte)( (HexToNumber(b1) << 4) | HexToNumber(b2) );

            static byte HexToNumber(byte b)
                => b switch
                {
                    >= (byte)'0' and <= (byte)'9' => (byte)( b - '0' ),
                    >= (byte)'A' and <= (byte)'F' => (byte)( b - 'A' + 10 ),
                    >= (byte)'a' and <= (byte)'f' => (byte)( b - 'a' + 10 ),
                    _ => throw new InvalidOperationException("All hex values should be covered.")
                };
        }
    }

    private static byte[] _escape(byte[] input)
    {
        var output = new List<byte>();

        foreach (var b in input)
        {
            switch (b)
            {
                case 0x00:
                    throw new InvalidOperationException("NULL character is not allowed in a pdf name.");
                case 0x23:
                    output.Add(0x23);
                    output.Add(0x32);
                    output.Add(0x33);
                    break;
                case byte b1 when ByteUtils.IsDelimiterorWhiteSpace(b1):
                case byte b2 when b2 < 33 || b2 > 126:
                    output.Add(0x23);
                    output.Add(ToHex(b >> 4));
                    output.Add(ToHex(b & 0x0F));
                    break;
                default:
                    output.Add(b);
                    break;
            }
        }

        return output.ToArray();

        static byte ToHex(int input)
            => input switch
            {
                int i when i <= 9 => (byte)( i + '0' ),
                int i => (byte)( i - 10 + 'A' )
            };
    }

    public static explicit operator PdfName(string name)
        => Get(name);

    public static bool operator ==(PdfName left, PdfName right)
        => left.Equals(right);

    public static bool operator !=(PdfName left, PdfName right)
        => !( left == right );
}
