using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Generation;

/// <summary>
/// Handles serialization of PDF primitives as direct objects to a stream
/// </summary>
public sealed class DirectObjectSerializer
{
    private readonly PdfStream _stream;

    /// <summary>
    /// Creates a new instance of DirectObjectSerializer
    /// </summary>
    /// <param name="stream">The stream to write to</param>
    public DirectObjectSerializer(PdfStream stream)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream));
    }

    /// <summary>
    /// Writes a PDF primitive as a direct object to the stream
    /// </summary>
    /// <param name="primitive">The primitive to write</param>
    /// <exception cref="ArgumentNullException">Thrown when primitive is null</exception>
    /// <exception cref="ArgumentException">Thrown when attempting to write a PDF stream as a direct object</exception>
    /// <exception cref="NotImplementedException">Thrown when the primitive type is not supported</exception>
    public void WriteDirect(IPdfPrimitive primitive)
    {
        if (primitive is null)
            throw new ArgumentNullException(nameof(primitive));

        switch (primitive)
        {
            case IPdfStreamObject:
                throw new ArgumentException("Pdf streams can not be written as direct objects.", nameof(primitive));
            case IPdfArray array:
                WriteDirect(array);
                break;
            case PdfName name:
                WriteDirect(name);
                break;
            case PdfBoolean pdfBool:
                WriteDirect(pdfBool);
                break;
            case PdfNull pdfNull:
                WriteDirect(pdfNull);
                break;
            case PdfNumber pdfNumber:
                WriteDirect(pdfNumber);
                break;
            case PdfString pdfString:
                WriteDirect(pdfString);
                break;
            case PdfReference pdfReference:
                WriteDirect(pdfReference);
                break;
            case IPdfDictionary pdfDictionary:
                WriteDirect(pdfDictionary);
                break;
            default:
                throw new NotImplementedException($"Serialization not implemented for type {primitive.GetType()}");
        }
    }

    /// <summary>
    /// Writes a PDF array as a direct object
    /// </summary>
    /// <param name="array">The array to write</param>
    public void WriteDirect(IPdfArray array)
    {
        _stream.WriteByte(ByteUtils.LEFT_SQUARE_BRACKET);

        for (int i = 0; i < array.Count; i++)
        {
            var value = array[i];

            if (i != 0 && value is PdfBoolean or PdfNull or PdfNumber or PdfReference)
                _stream.Space();

            WriteDirect(array[i]);
        }

        _stream.WriteByte(ByteUtils.RIGHT_SQUARE_BRACKET);
    }

    /// <summary>
    /// Writes a PDF name as a direct object
    /// </summary>
    /// <param name="name">The name to write</param>
    public void WriteDirect(PdfName name)
    {
        _stream.WriteByte(ByteUtils.SOLIDUS);
        _stream.Write(name.Raw);
    }

    /// <summary>
    /// Writes a PDF boolean as a direct object
    /// </summary>
    /// <param name="boolean">The boolean to write</param>
    public void WriteDirect(PdfBoolean boolean)
    {
        _stream.Write(boolean.Value);
    }

    /// <summary>
    /// Writes a PDF null as a direct object
    /// </summary>
    /// <param name="pdfNull">The null to write</param>
    public void WriteDirect(PdfNull pdfNull)
    {
        _stream.Write("null");
    }

    /// <summary>
    /// Writes a PDF number as a direct object
    /// </summary>
    /// <param name="number">The number to write</param>
    public void WriteDirect(PdfNumber number)
    {
        if (number.IsFractional)
            _stream.Write(number.Value);
        else
            _stream.Write(number.LongValue);
    }

    /// <summary>
    /// Writes a PDF string as a direct object
    /// </summary>
    /// <param name="pdfString">The string to write</param>
    public void WriteDirect(PdfString pdfString)
    {
        if (pdfString.IsHex)
        {
            _stream.WriteByte(ByteUtils.LESS_THAN_SIGN);
            _stream.Write(Convert.ToHexString(pdfString.Raw));
            _stream.WriteByte(ByteUtils.GREATER_THAN_SIGN);
        }
        else
        {
            _stream.WriteByte(ByteUtils.PARENTHESIS_OPEN);
            _stream.Write(pdfString.Raw);
            _stream.WriteByte(ByteUtils.PARENTHESIS_CLOSED);
        }
    }

    /// <summary>
    /// Writes a PDF reference as a direct object
    /// </summary>
    /// <param name="pdfReference">The reference to write</param>
    public void WriteDirect(PdfReference pdfReference)
    {
        _stream.Write(pdfReference.Id.ObjectNumber);
        _stream.Space();
        _stream.Write(pdfReference.Id.Generation);
        _stream.Space();
        _stream.Write('R');
    }

    /// <summary>
    /// Writes a PDF dictionary as a direct object
    /// </summary>
    /// <param name="pdfDictionary">The dictionary to write</param>
    public void WriteDirect(IPdfDictionary pdfDictionary)
    {
        _stream.WriteByte(ByteUtils.LESS_THAN_SIGN);
        _stream.WriteByte(ByteUtils.LESS_THAN_SIGN);

        foreach (var (key, value) in pdfDictionary)
        {
            WriteDirect(key);
            if (value is PdfBoolean or PdfNull or PdfNumber or PdfReference)
                _stream.Space();
            WriteDirect(value);
        }

        _stream.WriteByte(ByteUtils.GREATER_THAN_SIGN);
        _stream.WriteByte(ByteUtils.GREATER_THAN_SIGN);
    }
}