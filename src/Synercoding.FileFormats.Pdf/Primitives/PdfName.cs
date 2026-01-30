using Synercoding.FileFormats.Pdf.IO;
using System.Diagnostics;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF name primitive, which is an atomic symbol uniquely defined by a sequence of characters.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public sealed class PdfName : IPdfPrimitive, IEquatable<PdfName>
{
    private static readonly IDictionary<string, PdfName> _reservedNames = new Dictionary<string, PdfName>()
    {
        { "AESV2",            new PdfName("AESV2") },
        { "AESV3",            new PdfName("AESV3") },
        { "ArtBox",           new PdfName("ArtBox") },
        { "ASCII85Decode",    new PdfName("ASCII85Decode") },
        { "ASCIIHexDecode",   new PdfName("ASCIIHexDecode") },
        { "Ascent",           new PdfName("Ascent") },
        { "Author",           new PdfName("Author") },
        { "BaseFont",         new PdfName("BaseFont") },
        { "BitsPerComponent", new PdfName("BitsPerComponent") },
        { "Black",            new PdfName("Black") },
        { "BleedBox",         new PdfName("BleedBox") },
        { "CapHeight",        new PdfName("CapHeight") },
        { "Catalog",          new PdfName("Catalog") },
        { "CCITTFaxDecode",   new PdfName("CCITTFaxDecode") },
        { "CF",               new PdfName("CF") },
        { "CFM",              new PdfName("CFM") },
        { "CIDFontType2",     new PdfName("CIDFontType2") },
        { "CIDSystemInfo",    new PdfName("CIDSystemInfo") },
        { "CIDToGIDMap",      new PdfName("CIDToGIDMap") },
        { "ColorSpace",       new PdfName("ColorSpace") },
        { "Colors",           new PdfName("Colors") },
        { "Columns",          new PdfName("Columns") },
        { "Contents",         new PdfName("Contents") },
        { "Count",            new PdfName("Count") },
        { "CreationDate",     new PdfName("CreationDate") },
        { "Creator",          new PdfName("Creator") },
        { "CropBox",          new PdfName("CropBox") },
        { "Crypt",            new PdfName("Crypt") },
        { "Cyan",             new PdfName("Cyan") },
        { "DCTDecode",        new PdfName("DCTDecode") },
        { "Decode",           new PdfName("Decode") },
        { "DecodeParms",      new PdfName("DecodeParms") },
        { "DescendantFonts",  new PdfName("DescendantFonts") },
        { "Descent",          new PdfName("Descent") },
        { "DW",               new PdfName("DW") },
        { "DeviceCMYK",       new PdfName("DeviceCMYK") },
        { "DeviceGray",       new PdfName("DeviceGray") },
        { "DeviceRGB",        new PdfName("DeviceRGB") },
        { "EarlyChange",      new PdfName("EarlyChange") },
        { "Encoding",         new PdfName("Encoding") },
        { "EFF",              new PdfName("EFF") },
        { "Encrypt",          new PdfName("Encrypt") },
        { "EncryptMetadata",  new PdfName("EncryptMetadata") },
        { "ExtGState",        new PdfName("ExtGState") },
        { "Filter",           new PdfName("Filter") },
        { "First",            new PdfName("First") },
        { "Flags",            new PdfName("Flags") },
        { "FontBBox",         new PdfName("FontBBox") },
        { "FontDescriptor",   new PdfName("FontDescriptor") },
        { "FontFile2",        new PdfName("FontFile2") },
        { "FontName",         new PdfName("FontName") },
        { "FlateDecode",      new PdfName("FlateDecode") },
        { "Font",             new PdfName("Font") },
        { "FullScreen",       new PdfName("FullScreen") },
        { "Height",           new PdfName("Height") },
        { "ID",               new PdfName("ID") },
        { "Identity",         new PdfName("Identity") },
        { "Identity-H",       new PdfName("Identity-H") },
        { "Image",            new PdfName("Image") },
        { "Index",            new PdfName("Index") },
        { "Info",             new PdfName("Info") },
        { "ItalicAngle",      new PdfName("ItalicAngle") },
        { "JBIG2Decode",      new PdfName("JBIG2Decode") },
        { "JPXDecode",        new PdfName("JPXDecode") },
        { "Keywords",         new PdfName("Keywords") },
        { "Kids",             new PdfName("Kids") },
        { "Length",           new PdfName("Length") },
        { "Length1",          new PdfName("Length1") },
        { "LZWDecode",        new PdfName("LZWDecode") },
        { "Magenta",          new PdfName("Magenta") },
        { "MediaBox",         new PdfName("MediaBox") },
        { "Metadata",         new PdfName("Metadata") },
        { "ModDate",          new PdfName("ModDate") },
        { "N",                new PdfName("N") },
        { "None",             new PdfName("None") },
        { "O",                new PdfName("O") },
        { "ObjStm",           new PdfName("ObjStm") },
        { "OE",               new PdfName("OE") },
        { "OneColumn",        new PdfName("OneColumn") },
        { "OP",               new PdfName("OP") },
        { "op",               new PdfName("op") },
        { "Ordering",         new PdfName("Ordering") },
        { "P",                new PdfName("P") },
        { "Page",             new PdfName("Page") },
        { "PageLayout",       new PdfName("PageLayout") },
        { "PageMode",         new PdfName("PageMode") },
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
        { "Registry",         new PdfName("Registry") },
        { "Resources",        new PdfName("Resources") },
        { "Root",             new PdfName("Root") },
        { "Rotate",           new PdfName("Rotate") },
        { "RunLengthDecode",  new PdfName("RunLengthDecode") },
        { "Separation",       new PdfName("Separation") },
        { "Shading",          new PdfName("Shading") },
        { "SinglePage",       new PdfName("SinglePage") },
        { "Size",             new PdfName("Size") },
        { "SMask",            new PdfName("SMask") },
        { "Standard",         new PdfName("Standard") },
        { "StemV",            new PdfName("StemV") },
        { "StmF",             new PdfName("StmF") },
        { "StrF",             new PdfName("StrF") },
        { "SubFilter",        new PdfName("SubFilter") },
        { "Subject",          new PdfName("Subject") },
        { "Subtype",          new PdfName("Subtype") },
        { "Supplement",       new PdfName("Supplement") },
        { "Title",            new PdfName("Title") },
        { "ToUnicode",        new PdfName("ToUnicode") },
        { "TrimBox",          new PdfName("TrimBox") },
        { "TwoColumnLeft",    new PdfName("TwoColumnLeft") },
        { "TwoColumnRight",   new PdfName("TwoColumnRight") },
        { "TwoPageLeft",      new PdfName("TwoPageLeft") },
        { "TwoPageRight",     new PdfName("TwoPageRight") },
        { "Type",             new PdfName("Type") },
        { "Type0",            new PdfName("Type0") },
        { "Type1",            new PdfName("Type1") },
        { "U",                new PdfName("U") },
        { "UE",               new PdfName("UE") },
        { "UseAttachments",   new PdfName("UseAttachments") },
        { "UseNone",          new PdfName("UseNone") },
        { "UseOC",            new PdfName("UseOC") },
        { "UseOutlines",      new PdfName("UseOutlines") },
        { "UserUnit",         new PdfName("UserUnit") },
        { "UseThumbs",        new PdfName("UseThumbs") },
        { "V",                new PdfName("V") },
        { "V2",               new PdfName("V2") },
        { "V4",               new PdfName("V4") },
        { "V5",               new PdfName("V5") },
        { "Version",          new PdfName("Version") },
        { "W",                new PdfName("W") },
        { "Width",            new PdfName("Width") },
        { "XObject",          new PdfName("XObject") },
        { "XRef",             new PdfName("XRef") },
        { "XRefStm",          new PdfName("XRefStm") },
        { "Yellow",           new PdfName("Yellow") },
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

    /// <summary>
    /// Gets the display string representation of this PDF name.
    /// </summary>
    public string Display { get; }

    /// <inheritdoc />
    public bool Equals(PdfName? other)
        => other is not null && Raw.SequenceEqual(other.Raw);

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is PdfName pdfName && Equals(pdfName);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.AddBytes(Raw);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc />
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

    /// <summary>
    /// Explicitly converts a <see cref="string"/> to a <see cref="PdfName"/>.
    /// </summary>
    /// <param name="name">The name string to convert.</param>
    public static explicit operator PdfName(string name)
        => Get(name);

    /// <summary>
    /// Determines whether two <see cref="PdfName"/> instances are equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfName left, PdfName right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfName"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first instance to compare.</param>
    /// <param name="right">The second instance to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfName left, PdfName right)
        => !( left == right );
}
