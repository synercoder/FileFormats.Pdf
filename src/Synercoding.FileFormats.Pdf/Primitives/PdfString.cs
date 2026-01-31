using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing;
using System.Diagnostics;
using SystemEncoding = System.Text.Encoding;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a PDF string object, which can be either a literal string or hexadecimal string.
/// Supports multiple text encodings including UTF-16BE, UTF-16LE, UTF-8, PDFDocEncoding, and byte strings.
/// </summary>
[DebuggerDisplay("{ToString(),nq}")]
public class PdfString : IPdfPrimitive, IEquatable<PdfString>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PdfString"/> class.
    /// </summary>
    /// <param name="bytes">The raw byte array containing the string data.</param>
    /// <param name="isHex">True if this is a hexadecimal string, false if it's a literal string.</param>
    public PdfString(byte[] bytes, bool isHex)
    {
        Raw = bytes;
        IsHex = isHex;
    }

    internal byte[] Raw { get; }

    /// <summary>
    /// Gets the decoded string value using the appropriate encoding.
    /// </summary>
    public string Value
    {
        get
        {
            if (field is not null)
                return field;

            field = Encoding switch
            {
                PdfStringEncoding.Utf16BE => SystemEncoding.BigEndianUnicode.GetString(Raw, 2, Raw.Length - 2),
                PdfStringEncoding.Utf16LE => SystemEncoding.Unicode.GetString(Raw, 2, Raw.Length - 2),
                PdfStringEncoding.Utf8 => SystemEncoding.UTF8.GetString(Raw, 3, Raw.Length - 3),
                PdfStringEncoding.PdfDocEncoding => PDFDocEncoding.Decode(Raw),
                PdfStringEncoding.ByteString => '<' + Convert.ToHexString(Raw) + '>',
                var unknown => throw new NotImplementedException($"Unknown {nameof(PdfStringEncoding)} value: {unknown}.")
            };

            return field;
        }
    }

    /// <summary>
    /// Gets the encoding of this PDF string based on byte order marks or content analysis.
    /// </summary>
    public PdfStringEncoding Encoding
    {
        get
        {
            return Raw switch
            {
                [0xFE, 0xFF, ..] => PdfStringEncoding.Utf16BE,
                [0xFF, 0xFE, ..] => PdfStringEncoding.Utf16LE,
                [0xEF, 0xBB, 0xBF, ..] => PdfStringEncoding.Utf8,
                _ when PDFDocEncoding.CanDecode(Raw) => PdfStringEncoding.PdfDocEncoding,
                _ when IsHex => PdfStringEncoding.ByteString,
                _ => throw new ParseException("Could not determine the encoding of the PdfString.")
            };
        }
    }

    /// <summary>
    /// Gets a value indicating whether this string is encoded as hexadecimal.
    /// </summary>
    public bool IsHex { get; }

    /// <summary>
    /// Determines whether the specified <see cref="PdfString"/> is equal to the current instance.
    /// </summary>
    /// <param name="other">The <see cref="PdfString"/> to compare with the current instance.</param>
    /// <returns>True if the specified <see cref="PdfString"/> is equal to the current instance; otherwise, false.</returns>
    public bool Equals(PdfString? other)
    {
        if (other is null)
            return false;

        return IsHex == other.IsHex
            && Encoding == other.Encoding
            && Value == other.Value;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>True if the specified object is equal to the current instance; otherwise, false.</returns>
    public override bool Equals(object? obj)
        => Equals(obj as PdfString);

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
        => HashCode.Combine(IsHex, Encoding, Value);

    /// <summary>
    /// Returns a string representation of this PDF string for debugging purposes.
    /// </summary>
    /// <returns>A string representation of this PDF string.</returns>
    public override string ToString()
        => $"[Pdf {( IsHex ? "hex" : "literal" )} {Encoding} string] \"{Value}\"";

    /// <summary>
    /// Determines whether two <see cref="PdfString"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="PdfString"/> to compare.</param>
    /// <param name="right">The second <see cref="PdfString"/> to compare.</param>
    /// <returns>True if the instances are equal; otherwise, false.</returns>
    public static bool operator ==(PdfString left, PdfString right)
        => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="PdfString"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="PdfString"/> to compare.</param>
    /// <param name="right">The second <see cref="PdfString"/> to compare.</param>
    /// <returns>True if the instances are not equal; otherwise, false.</returns>
    public static bool operator !=(PdfString left, PdfString right)
        => !( left == right );
}
