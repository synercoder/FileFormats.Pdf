using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Parsing;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Globalization;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Generation;

/// <summary>
/// A stream wrapper for writing PDF content with built-in methods for writing PDF data types.
/// </summary>
public sealed class PdfStream : IDisposable
{
    private const string NUMBER_STRING_FORMAT = "0.0########";

    private readonly Stream _innerStream;
    private readonly bool _ownsStream;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfStream"/> class.
    /// </summary>
    /// <param name="stream">The underlying stream to write to.</param>
    public PdfStream(Stream stream)
        : this(stream, false)
    { }

    /// <summary>
    /// Constructor for <see cref="PdfStream"/>
    /// </summary>
    /// <param name="stream">The stream where everything is written to.</param>
    /// <param name="ownsStream">Will dispose of the provided <paramref name="stream"/> when true.</param>
    public PdfStream(Stream stream, bool ownsStream)
    {
        if (!stream.CanWrite)
            throw new ArgumentException("Provided stream must be writable.", nameof(stream));

        _innerStream = stream;
        _ownsStream = ownsStream;
    }

    /// <summary>
    /// Get the position in the stream
    /// </summary>
    public long Position
        => _innerStream.Position;

    internal IPdfStreamObject ToStreamObject(params IStreamFilter[] filters)
    {
        if (filters.Any(f => f.PassThrough))
            throw new ArgumentException("Provided stream filter is passthrough, and can not be used to encode data.", nameof(filters));

        // Get the stream data
        _innerStream.Flush();

        byte[] rawData;
        if (_innerStream is MemoryStream ms)
        {
            rawData = ms.ToArray();
        }
        else
        {
            var currentPosition = _innerStream.Position;
            _innerStream.Position = 0;
            using (var memoryStream = new MemoryStream())
            {
                _innerStream.CopyTo(memoryStream);
                rawData = memoryStream.ToArray();
            }
            _innerStream.Position = currentPosition;
        }

        foreach (var filter in filters)
            rawData = filter.Encode(rawData, null);

        var dictionary = new PdfDictionary()
        {
            [PdfNames.Length] = new PdfNumber(rawData.Length)
        };
        if (filters.Any())
        {
            var filtersNames = filters
                .Select(f => f.Name)
                .ToArray();

            if (filtersNames.Length == 1)
                dictionary.Add(PdfNames.Filter, filtersNames.Single());
            else
                dictionary.Add(PdfNames.Filter, new PdfArray(filtersNames));
        }

        // Create a stream object with the raw data
        return new Primitives.Internal.ReadOnlyPdfStreamObject(dictionary, rawData);
    }


    /// <summary>
    /// Write a space ' ' character to the stream.
    /// </summary>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Space()
        => Write(' ');

    /// <summary>
    /// Write a \r\n newline to the stream.
    /// </summary>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream NewLine()
        => Write('\r').Write('\n');

    /// <summary>
    /// Write a <see cref="byte"/> to the stream.
    /// </summary>
    /// <param name="b">The byte to write.</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream WriteByte(byte b)
    {
        _innerStream.WriteByte(b);

        return this;
    }

    /// <summary>
    /// Write a <see cref="char"/> to the stream
    /// </summary>
    /// <param name="c">The char to write.</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(char c)
    {
        if (c > 0xFF)
            throw new InvalidOperationException($"Char {c} cannot be encoded directly because it is outside of the default ASCII range.");

        return WriteByte((byte)c);
    }

    /// <summary>
    /// Write a <see cref="bool"/> to the stream
    /// </summary>
    /// <param name="b">The boolean to write.</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(bool b)
    {
        return b
            ? Write("true")
            : Write("false");
    }

    /// <summary>
    /// Write a <see cref="int"/> to the stream
    /// </summary>
    /// <param name="value">The integer to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(int value)
    {
        if (value < 0)
        {
            Write('-');
            value *= -1;
        }

        var intSize = ByteSizes.Size(value);
        for (int i = intSize - 1; i >= 0; i--)
        {
            var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
            _innerStream.WriteByte(result);
        }

        return this;
    }

    /// <summary>
    /// Write a <see cref="long"/> to the stream
    /// </summary>
    /// <param name="value">The long to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(long value)
    {
        if (value < 0)
        {
            _innerStream.WriteByte((byte)'-');
            value *= -1;
        }

        var intSize = ByteSizes.Size(value);
        for (int i = intSize - 1; i >= 0; i--)
        {
            var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
            _innerStream.WriteByte(result);
        }

        return this;
    }

    /// <summary>
    /// Write a <see cref="double"/> to the stream
    /// </summary>
    /// <param name="value">The double to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(double value)
    {
        var stringValue = value.ToString(NUMBER_STRING_FORMAT, CultureInfo.InvariantCulture);
        var bytes = Encoding.ASCII.GetBytes(stringValue);
        _innerStream.Write(bytes, 0, bytes.Length);

        return this;
    }

    /// <summary>
    /// Write a <see cref="string"/> to the stream
    /// </summary>
    /// <param name="text">The string to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(string text)
    {
        var bytes = Encoding.ASCII.GetBytes(text);
        _innerStream.Write(bytes, 0, bytes.Length);

        return this;
    }

    /// <summary>
    /// Copy bytes to the stream from a buffer
    /// </summary>
    /// <param name="buffer">The byte buffer to write</param>
    /// <param name="offset">The offset in the array to start copying</param>
    /// <param name="count">The amount of bytes to copy</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);

        return this;
    }

    /// <summary>
    /// Write a <see cref="ReadOnlySpan{T}"/> of type <see cref="byte"/> to the stream
    /// </summary>
    /// <param name="buffer">The buffer to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(ReadOnlySpan<byte> buffer)
    {
        _innerStream.Write(buffer);

        return this;
    }

    /// <summary>
    /// Write a text to the stream as a string literal
    /// </summary>
    /// <param name="value">The string to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    /// <exception cref="InvalidOperationException">Throws when the string contains characters outside of the StandardEncoding range.</exception>
    internal PdfStream WriteStringLiteral(string value)
    {
        WriteByte(0x28); // (

        var bytes = PDFDocEncoding.CanEncode(value)
            ? PDFDocEncoding.Encode(value)
            : [.. Encoding.UTF8.Preamble, .. Encoding.UTF8.GetBytes(value)];

        foreach (var b in bytes)
        {
            if (b == '(')
                Write('\\').Write('(');
            else if (b == ')')
                Write('\\').Write(')');
            else if (b == '\\')
                Write('\\').Write('\\');
            else
                Write(b);
        }

        WriteByte(0x29); // )

        return this;
    }

    /// <summary>
    /// Write a text to the stream as a string literal
    /// </summary>
    /// <param name="encodedString">The string to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream WriteStringLiteral(byte[] encodedString)
    {
        WriteByte(0x28); // (

        foreach (var b in encodedString)
        {
            if (b == '(')
                Write('\\').Write('(');
            else if (b == ')')
                Write('\\').Write(')');
            else if (b == '\\')
                Write('\\').Write('\\');
            else
                WriteByte(b);
        }

        WriteByte(0x29); // )

        return this;
    }

    /// <summary>
    /// Write an array of numbers to the pdf stream
    /// </summary>
    /// <param name="array">The array of doubles to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    internal PdfStream Write(double[] array)
    {
        WriteByte(ByteUtils.LEFT_SQUARE_BRACKET)
            .Space();

        foreach (var number in array)
            Write(number).Space();

        WriteByte(ByteUtils.RIGHT_SQUARE_BRACKET);

        return this;
    }

    /// <summary>
    /// Flush the inner stream
    /// </summary>
    public void Flush()
    {
        _innerStream.Flush();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsStream)
            _innerStream.Dispose();
        GC.SuppressFinalize(this);
    }
}
