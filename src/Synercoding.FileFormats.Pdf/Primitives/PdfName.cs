using Synercoding.FileFormats.Pdf.IO;
using System.Diagnostics;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("[PdfName] {ToString(),nq}")]
public sealed class PdfName : IPdfPrimitive, IEquatable<PdfName>
{
    private static readonly IDictionary<string, PdfName> _reservedNames = new Dictionary<string, PdfName>()
    {
        { "ArtBox",           new PdfName("ArtBox") },
        { "ASCII85Decode",    new PdfName("ASCII85Decode") },
        { "ASCIIHexDecode",   new PdfName("ASCIIHexDecode") },
        { "Author",           new PdfName("Author") },
        { "BitsPerComponent", new PdfName("BitsPerComponent") },
        { "BleedBox",         new PdfName("BleedBox") },
        { "Catalog",          new PdfName("Catalog") },
        { "CF",               new PdfName("CF") },
        { "ColorSpace",       new PdfName("ColorSpace") },
        { "Colors",           new PdfName("Colors") },
        { "Columns",          new PdfName("Columns") },
        { "Contents",         new PdfName("Contents") },
        { "Count",            new PdfName("Count") },
        { "CreationDate",     new PdfName("CreationDate") },
        { "Creator",          new PdfName("Creator") },
        { "CropBox",          new PdfName("CropBox") },
        { "DCTDecode",        new PdfName("DCTDecode") },
        { "Decode",           new PdfName("Decode") },
        { "DecodeParms",      new PdfName("DecodeParms") },
        { "DeviceCMYK",       new PdfName("DeviceCMYK") },
        { "DeviceGray",       new PdfName("DeviceGray") },
        { "DeviceRGB",        new PdfName("DeviceRGB") },
        { "EarlyChange",      new PdfName("EarlyChange") },
        { "EFF",              new PdfName("EFF") },
        { "Encrypt",          new PdfName("Encrypt") },
        { "EncryptMetadata",  new PdfName("EncryptMetadata") },
        { "ExtGState",        new PdfName("ExtGState") },
        { "Filter",           new PdfName("Filter") },
        { "First",            new PdfName("First") },
        { "FlateDecode",      new PdfName("FlateDecode") },
        { "Font",             new PdfName("Font") },
        { "Height",           new PdfName("Height") },
        { "ID",               new PdfName("ID") },
        { "Identity",         new PdfName("Identity") },
        { "Image",            new PdfName("Image") },
        { "Index",            new PdfName("Index") },
        { "Info",             new PdfName("Info") },
        { "Keywords",         new PdfName("Keywords") },
        { "Kids",             new PdfName("Kids") },
        { "Length",           new PdfName("Length") },
        { "LZWDecode",        new PdfName("LZWDecode") },
        { "MediaBox",         new PdfName("MediaBox") },
        { "ModDate",          new PdfName("ModDate") },
        { "N",                new PdfName("N") },
        { "O",                new PdfName("O") },
        { "ObjStm",           new PdfName("ObjStm") },
        { "OE",               new PdfName("OE") },
        { "P",                new PdfName("P") },
        { "Page",             new PdfName("Page") },
        { "Pages",            new PdfName("Pages") },
        { "Parent",           new PdfName("Parent") },
        { "Pattern",          new PdfName("Pattern") },
        { "Perms",            new PdfName("Perms") },
        { "Predictor",        new PdfName("Predictor") },
        { "Prev",             new PdfName("Prev") },
        { "ProcSet",          new PdfName("ProcSet") },
        { "Producer",         new PdfName("Producer") },
        { "Properties",       new PdfName("Properties") },
        { "R",                new PdfName("R") },
        { "Resources",        new PdfName("Resources") },
        { "Root",             new PdfName("Root") },
        { "Rotate",           new PdfName("Rotate") },
        { "RunLengthDecode",  new PdfName("RunLengthDecode") },
        { "Shading",          new PdfName("Shading") },
        { "Size",             new PdfName("Size") },
        { "Standard",         new PdfName("Standard") },
        { "StmF",             new PdfName("StmF") },
        { "StrF",             new PdfName("StrF") },
        { "SubFilter",        new PdfName("SubFilter") },
        { "Subject",          new PdfName("Subject") },
        { "Subtype",          new PdfName("Subtype") },
        { "Title",            new PdfName("Title") },
        { "TrimBox",          new PdfName("TrimBox") },
        { "Type",             new PdfName("Type") },
        { "UserUnit",         new PdfName("UserUnit") },
        { "U",                new PdfName("U") },
        { "UE",               new PdfName("UE") },
        { "V",                new PdfName("V") },
        { "W",                new PdfName("W") },
        { "Width",            new PdfName("Width") },
        { "XObject",          new PdfName("XObject") },
        { "XRef",             new PdfName("XRef") },
        { "XRefStm",          new PdfName("XRefStm") },
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
        if (bytes == null)
            throw new ArgumentNullException(nameof(bytes));
        if (bytes.Length == 0)
            throw new ArgumentException("Provided byte array must not be empty.", nameof(bytes));

        Raw = bytes ?? throw new ArgumentNullException(nameof(bytes));
        Display = Encoding.UTF8.GetString(_unescape(bytes));
    }

    private PdfName(string input)
    {
        if (input is null)
            throw new ArgumentNullException(nameof(input));
        if (input == string.Empty)
            throw new ArgumentException("Provided name must not be empty.", nameof(input));

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
        => $"/{Encoding.UTF8.GetString(Raw)}";

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
            return (byte)( ( HexToNumber(b1) << 4 ) | HexToNumber(b2) );

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
                case byte b1 when ByteUtils.IsDelimiterOrWhiteSpace(b1):
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
