using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Diagnostics;
using SystemEncoding = System.Text.Encoding;

namespace Synercoding.FileFormats.Pdf.Primitives;

[DebuggerDisplay("{ToString(),nq}")]
public class PdfString : IPdfPrimitive, IEquatable<PdfString>
{
    public PdfString(byte[] bytes, bool isHex)
    {
        _bytes = bytes;
        IsHex = isHex;
    }

    private readonly byte[] _bytes;

    private string? _value;
    public string Value
    {
        get
        {
            if (_value is not null)
                return _value;

            _value = Encoding switch
            {
                PdfStringEncoding.Utf16BE => SystemEncoding.BigEndianUnicode.GetString(_bytes, 2, _bytes.Length - 2),
                PdfStringEncoding.Utf16LE => SystemEncoding.Unicode.GetString(_bytes, 2, _bytes.Length - 2),
                PdfStringEncoding.Utf8 => SystemEncoding.UTF8.GetString(_bytes, 3, _bytes.Length - 3),
                PdfStringEncoding.PdfDocEncoding => PDFDocEncoding.Decode(_bytes),
                PdfStringEncoding.ByteString => '<' + Convert.ToHexString(_bytes) + '>',
                var unknown => throw new NotImplementedException($"Unknown {nameof(PdfStringEncoding)} value: {unknown}.")
            };

            return _value;
        }
    }

    public PdfStringEncoding Encoding
    {
        get
        {
            return _bytes switch
            {
                [0xFE, 0xFF, ..] => PdfStringEncoding.Utf16BE,
                [0xFF, 0xFE, ..] => PdfStringEncoding.Utf16LE,
                [0xEF, 0xBB, 0xBF, ..] => PdfStringEncoding.Utf8,
                _ when PDFDocEncoding.CanDecode(_bytes) => PdfStringEncoding.PdfDocEncoding,
                _ when IsHex => PdfStringEncoding.ByteString,
                _ => throw new ParseException("Could not determine the encoding of the PdfString.")
            };
        }
    }

    public bool IsHex { get; }

    public bool Equals(PdfString? other)
    {
        if (other is null)
            return false;

        return IsHex == other.IsHex
            && Encoding == other.Encoding
            && Value == other.Value;
    }

    public override bool Equals(object? obj)
        => Equals(obj as PdfString);

    public override int GetHashCode()
        => HashCode.Combine(IsHex, Encoding, Value);

    public override string ToString()
        => $"[Pdf {( IsHex ? "hex" : "literal" )} {Encoding} string] \"{Value}\"";

    public static bool operator ==(PdfString left, PdfString right)
        => left.Equals(right);

    public static bool operator !=(PdfString left, PdfString right)
        => !( left == right );
}
