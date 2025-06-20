using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Globalization;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Generation;

public sealed class PdfStream
{
    private const string NUMBER_STRING_FORMAT = "0.0########";

    private readonly Stream _innerStream;
    private readonly bool _ownsStream;

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

    public PdfStream WriteDirect(IPdfPrimitive primitive)
    {
        if (primitive is null)
            throw new ArgumentNullException(nameof(primitive));

        return primitive switch
        {
            IPdfStreamObject => throw new ArgumentException("Pdf streams can not be written as direct objects.", nameof(primitive)),
            IPdfArray array => WriteDirect(array),
            PdfName name => WriteDirect(name),
            PdfBoolean pdfBool => WriteDirect(pdfBool),
            PdfNull pdfNull => WriteDirect(pdfNull),
            PdfNumber pdfNumber => WriteDirect(pdfNumber),
            PdfString pdfString => WriteDirect(pdfString),
            PdfReference pdfReference => WriteDirect(pdfReference),
            IPdfDictionary pdfDictionary => WriteDirect(pdfDictionary),
            _ => throw new NotImplementedException()
        };
    }

    public PdfStream WriteDirect(IPdfArray array)
    {
        Write(ByteUtils.LEFT_SQUARE_BRACKET);

        for (int i = 0; i < array.Count; i++)
        {
            WriteDirect(array[i]);
            if (i < array.Count - 1)
                Space();
        }

        Write(ByteUtils.RIGHT_SQUARE_BRACKET);

        return this;
    }

    public PdfStream WriteDirect(PdfName name)
    {
        return Write(ByteUtils.SOLIDUS)
            .Write(name.Raw);
    }

    public PdfStream WriteDirect(PdfBoolean boolean)
        => Write(boolean.Value);

    public PdfStream WriteDirect(PdfNull pdfNull)
        => Write("null");

    public PdfStream WriteDirect(PdfNumber number)
    {
        return number.IsFractional
            ? Write(number.Value)
            : Write(number.LongValue);
    }

    public PdfStream WriteDirect(PdfString pdfString)
    {
        if (pdfString.IsHex)
        {
            return Write(ByteUtils.LESS_THAN_SIGN)
                .Write(Convert.ToHexString(pdfString.Raw))
                .Write(ByteUtils.GREATER_THAN_SIGN);
        }

        return Write(ByteUtils.PARENTHESIS_OPEN)
            .Write(pdfString.Raw)
            .Write(ByteUtils.PARENTHESIS_CLOSED);
    }

    public PdfStream WriteDirect(PdfReference pdfReference)
    {
        return Write(pdfReference.Id.ObjectNumber)
            .Space()
            .Write(pdfReference.Id.Generation)
            .Space()
            .Write('R');
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
