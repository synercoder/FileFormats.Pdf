using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a read-only PDF dictionary primitive.
/// </summary>
public interface IPdfDictionary : IPdfPrimitive, IEnumerable<KeyValuePair<PdfName, IPdfPrimitive>>
{
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or null if not found.</returns>
    IPdfPrimitive? this[PdfName key] { get; }

    /// <summary>
    /// Gets a collection containing the keys in the dictionary.
    /// </summary>
    ICollection<PdfName> Keys { get; }

    /// <summary>
    /// Gets the number of key/value pairs in the dictionary.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines whether the dictionary contains the specified key.
    /// </summary>
    /// <param name="key">The key to locate.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    bool ContainsKey(PdfName key);

    /// <summary>
    /// Attempts to get the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value if found; otherwise, null.</param>
    /// <returns>True if the dictionary contains an element with the specified key; otherwise, false.</returns>
    bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value);

    /// <summary>
    /// Attempts to get the value associated with the specified key as the specified type.
    /// </summary>
    /// <typeparam name="TPrimitive">The expected type of the value.</typeparam>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">When this method returns, contains the value if found and of the correct type; otherwise, null.</param>
    /// <returns>True if the dictionary contains an element with the specified key and it is of the correct type; otherwise, false.</returns>
    bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive;
}
