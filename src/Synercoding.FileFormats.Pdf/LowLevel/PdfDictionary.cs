using Synercoding.FileFormats.Pdf.LowLevel.Extensions;

namespace Synercoding.FileFormats.Pdf.LowLevel;

/// <summary>
/// Class represents a dictionary in a pdf
/// </summary>
public class PdfDictionary
{
    private readonly PdfStream _stream;

    /// <summary>
    /// Constructor for <see cref="PdfDictionary"/>
    /// </summary>
    /// <param name="stream">The stream to </param>
    public PdfDictionary(PdfStream stream)
    {
        _stream = stream;
    }

    /// <summary>
    /// Write an array of numbers to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="numbers">The array to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, params int[] numbers)
    {
        _stream
            .Write(key)
            .Space()
            .Write(numbers);

        return this;
    }

    /// <summary>
    /// Write an array of numbers to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="numbers">The array to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, params double[] numbers)
    {
        _stream
            .Write(key)
            .Space()
            .Write(numbers);

        return this;
    }

    /// <summary>
    /// Write an array of numbers to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="numbers">The array to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, params float[] numbers)
    {
        _stream
            .Write(key)
            .Space()
            .Write(numbers);

        return this;
    }

    /// <summary>
    /// Write an array of <see cref="PdfName"/> to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="names">The array to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, params PdfName[] names)
    {
        _stream
            .Write(key)
            .Space()
            .Write(names);

        return this;
    }

    /// <summary>
    /// Write an array of <see cref="PdfReference"/>s to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="objectReferences">The array to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, params PdfReference[] objectReferences)
    {
        _stream
            .Write(key)
            .Space()
            .Write(objectReferences);

        return this;
    }

    /// <summary>
    /// Write a number to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The number to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, double value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a number to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The number to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, long value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a number to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The number to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, int value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a boolean to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The boolean to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, bool value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a text to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The text to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, string value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a text to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The text to write as a literal string</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteLiteralString(PdfName key, string value)
    {
        _stream
            .Write(key)
            .Space()
            .WriteStringLiteral(value);

        return this;
    }

    /// <summary>
    /// Write a text to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The text to write as a hexadecimal string</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteHexadecimalString(PdfName key, string value)
    {
        _stream
            .Write(key)
            .Space()
            .WriteHexadecimalString(value);

        return this;
    }

    /// <summary>
    /// Write a date to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The date to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, DateTimeOffset value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }

    /// <summary>
    /// Write a <see cref="PdfName"/> to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The <see cref="PdfName"/> to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, PdfName value)
    {
        _stream
            .Write(key)
            .Space()
            .Write(value);

        return this;
    }


    /// <summary>
    /// Write a <see cref="PdfReference"/> to the dictionary
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="objectReference">The <see cref="PdfReference"/> to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, PdfReference objectReference)
    {
        _stream
            .Write(key)
            .Space()
            .Write(objectReference);

        return this;
    }

    /// <summary>
    /// Use an action to write things to the stream
    /// </summary>
    /// <typeparam name="T">The type of data to pass to <paramref name="rawActions"/></typeparam>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="data">Data to use in the <paramref name="rawActions"/></param>
    /// <param name="rawActions">The action to use to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write<T>(PdfName key, T data, Action<T, PdfStream> rawActions)
    {
        _stream
            .Write(key)
            .Space();

        rawActions(data, _stream);

        return this;
    }

    /// <summary>
    /// Use an action to write things to the stream
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="rawActions">The action to use to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, Action<PdfStream> rawActions)
        => Write(key, rawActions, static (action, stream) => action(stream));

    /// <summary>
    /// Write a <see cref="Rectangle"/> to the stream
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="rectangle">The <see cref="Rectangle"/> to write.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary Write(PdfName key, Rectangle rectangle)
    {
        _stream
            .Write(key)
            .Space()
            .Write(rectangle);

        return this;
    }

    /// <summary>
    /// Write a rectangle to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="rectangle">The <see cref="Rectangle"/> to write.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteIfNotNull(PdfName key, Rectangle? rectangle)
        => rectangle.HasValue
        ? Write(key, rectangle.Value)
        : this;

    public PdfDictionary WriteIfNotNull(PdfName key, PdfReference? value)
        => value.HasValue
        ? Write(key, value.Value)
        : this;

    /// <summary>
    /// Write a number to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The number to write.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteIfNotNull(PdfName key, int? value)
        => value.HasValue
        ? Write(key, value.Value)
        : this;

    /// <summary>
    /// Write a number to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The number to write.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteIfNotNull(PdfName key, double? value)
        => value.HasValue
        ? Write(key, value.Value)
        : this;

    /// <summary>
    /// Write a date to the dictionary if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The date to write</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteIfNotNull(PdfName key, DateTimeOffset? value)
        => value.HasValue
        ? Write(key, value.Value)
        : this;

    /// <summary>
    /// Write a number to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The string to write.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteIfNotNullOrWhiteSpace(PdfName key, string? value)
        => !string.IsNullOrWhiteSpace(value)
        ? Write(key, value)
        : this;

    /// <summary>
    /// Write a number to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The string to write as a pdf literal string.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteLiteralIfNotNullOrWhiteSpace(PdfName key, string? value)
        => !string.IsNullOrWhiteSpace(value)
        ? WriteLiteralString(key, value)
        : this;

    /// <summary>
    /// Write a number to the stream if it is not null
    /// </summary>
    /// <param name="key">The key of the item in the dictionary</param>
    /// <param name="value">The string to write as a pdf hexadecimal string.</param>
    /// <returns>The <see cref="PdfDictionary"/> to support chaining operations.</returns>
    public PdfDictionary WriteHexadecimalIfNotNullOrWhiteSpace(PdfName key, string? value)
        => !string.IsNullOrWhiteSpace(value)
        ? WriteHexadecimalString(key, value)
        : this;
}
