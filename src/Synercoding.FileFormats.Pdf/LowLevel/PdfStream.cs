using Synercoding.FileFormats.Pdf.Internals;
using System.Globalization;
using System.Text;

namespace Synercoding.FileFormats.Pdf.LowLevel;

/// <summary>
/// Class that represents a wrapper around a stream to make writing PDF instructions easier.
/// </summary>
public class PdfStream : IDisposable
{
    private const string NUMBER_STRING_FORMAT = "0.0########";

    /// <summary>
    /// Constructor for <see cref="PdfStream"/>
    /// </summary>
    /// <param name="stream">The stream where everything is written to.</param>
    public PdfStream(Stream stream)
    {
        InnerStream = stream;
    }

    /// <summary>
    /// Get the position in the stream
    /// </summary>
    public uint Position
        => (uint)InnerStream.Position;

    /// <summary>
    /// The stream that is wrapped an being written to
    /// </summary>
    protected internal Stream InnerStream { get; }

    /// <summary>
    /// Write a <see cref="byte"/> to the stream.
    /// </summary>
    /// <param name="b">The byte to write.</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream WriteByte(byte b)
    {
        InnerStream.WriteByte(b);

        return this;
    }

    /// <summary>
    /// Write a <see cref="char"/> to the stream
    /// </summary>
    /// <param name="c">The char to write.</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(char c)
    {
        if (c > 0xFF)
            throw new InvalidOperationException($"Char {c} cannot be encoded directly because it is outside of the default ASCII range.");

        return WriteByte((byte)( c & 0xFF ));
    }

    /// <summary>
    /// Write a <see cref="int"/> to the stream
    /// </summary>
    /// <param name="value">The integer to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(int value)
    {
        var intSize = ByteSizes.Size(value);
        for (int i = intSize - 1; i >= 0; i--)
        {
            var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
            InnerStream.WriteByte(result);
        }

        return this;
    }

    /// <summary>
    /// Write a <see cref="long"/> to the stream
    /// </summary>
    /// <param name="value">The long to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(long value)
    {
        var intSize = ByteSizes.Size(value);
        for (int i = intSize - 1; i >= 0; i--)
        {
            var result = (byte)( '0' + ( (int)( value / Math.Pow(10, i) ) % 10 ) );
            InnerStream.WriteByte(result);
        }

        return this;
    }

    /// <summary>
    /// Write a <see cref="double"/> to the stream
    /// </summary>
    /// <param name="value">The double to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(double value)
    {
        var stringValue = value.ToString(NUMBER_STRING_FORMAT, CultureInfo.InvariantCulture);
        var bytes = Encoding.ASCII.GetBytes(stringValue);
        InnerStream.Write(bytes, 0, bytes.Length);

        return this;
    }

    /// <summary>
    /// Write a <see cref="string"/> to the stream
    /// </summary>
    /// <param name="text">The string to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(string text)
    {
        var bytes = Encoding.ASCII.GetBytes(text);
        InnerStream.Write(bytes, 0, bytes.Length);

        return this;
    }

    /// <summary>
    /// Copy bytes to the stream from a buffer
    /// </summary>
    /// <param name="buffer">The byte buffer to write</param>
    /// <param name="offset">The offset in the array to start copying</param>
    /// <param name="count">The amount of bytes to copy</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(byte[] buffer, int offset, int count)
    {
        InnerStream.Write(buffer, offset, count);

        return this;
    }

    /// <summary>
    /// Write a <see cref="ReadOnlySpan{T}"/> of type <see cref="byte"/> to the stream
    /// </summary>
    /// <param name="buffer">The buffer to write</param>
    /// <returns>The calling <see cref="PdfStream"/> to support chaining operations.</returns>
    public PdfStream Write(ReadOnlySpan<byte> buffer)
    {
        InnerStream.Write(buffer);

        return this;
    }

    /// <summary>
    /// Flush the inner stream
    /// </summary>
    public void Flush()
    {
        InnerStream.Flush();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Parameter to indicate wheter this method was called from dispose or finalizer</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            InnerStream.Dispose();
        }
    }
}
