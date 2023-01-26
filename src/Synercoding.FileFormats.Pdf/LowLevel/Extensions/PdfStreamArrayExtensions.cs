namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions;

/// <summary>
/// Extensions methods for <see cref="PdfStream"/>
/// </summary>
public static class PdfStreamArrayExtensions
{
    private const byte BRACKET_OPEN = 0x5B;  // [
    private const byte BRACKET_CLOSE = 0x5D; // ]

    /// <summary>
    /// Write an array of chars to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="array">The array of chars to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, params char[] array)
    {
        stream
            .WriteByte(BRACKET_OPEN)
            .Space();

        foreach (var c in array)
            stream.Write(c).Space();

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }

    /// <summary>
    /// Write an array of numbers to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="array">The array of doubles to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, params double[] array)
    {
        stream
            .WriteByte(BRACKET_OPEN)
            .Space();

        foreach (var number in array)
            stream.Write(number).Space();

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }

    /// <summary>
    /// Write an array of numbers to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="array">The array of floats to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, params float[] array)
    {
        stream
            .WriteByte(BRACKET_OPEN)
            .Space();

        foreach (var number in array)
            stream.Write(number).Space();

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }

    /// <summary>
    /// Write an array of <see cref="PdfName"/> to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="array">The array of <see cref="PdfName"/> to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, params PdfName[] array)
    {
        stream
            .WriteByte(BRACKET_OPEN)
            .Space();

        foreach (var name in array)
            stream.Write(name).Space();

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }

    /// <summary>
    /// Write an array of numbers to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="array">The array of integers to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, int[] array)
    {
        stream
            .WriteByte(BRACKET_OPEN)
            .Space();

        foreach (var number in array)
            stream.Write(number).Space();

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }

    /// <summary>
    /// Write an array of pdf references to the pdf stream
    /// </summary>
    /// <param name="stream">The pdf stream to write the array to.</param>
    /// <param name="objectReferences">The array of <see cref="PdfReference"/> to write</param>
    /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
    public static PdfStream Write(this PdfStream stream, PdfReference[] objectReferences)
    {
        stream.WriteByte(BRACKET_OPEN).Space();

        foreach (var objectReference in objectReferences)
        {
            stream.Write(objectReference);
            stream.Space();
        }

        stream.WriteByte(BRACKET_CLOSE);

        return stream;
    }
}
