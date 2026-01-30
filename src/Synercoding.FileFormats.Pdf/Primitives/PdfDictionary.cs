using Synercoding.FileFormats.Pdf.Exceptions;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a mutable PDF dictionary that maps names to PDF primitive values.
/// </summary>
[DebuggerDisplay("[Pdf Dictionary] {ToString(),nq}")]
public class PdfDictionary : IPdfDictionary
{
    private readonly Dictionary<PdfName, IPdfPrimitive> _dictionary = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfDictionary"/> class.
    /// </summary>
    public PdfDictionary()
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfDictionary"/> class by copying from another dictionary.
    /// </summary>
    /// <param name="dictionary">The dictionary to copy from.</param>
    public PdfDictionary(IPdfDictionary dictionary)
    {
        foreach (var (key, value) in dictionary)
            _dictionary.Add(key, value);
    }

    /// <inheritdoc />
    public IPdfPrimitive? this[PdfName key]
    {
        get => _dictionary[key];
        set
        {
            ArgumentNullException.ThrowIfNull(key);

            // Pdf 2.0 Ref 7.3.7: A dictionary entry whose value is null (see 7.3.9, "Null object")
            // shall be treated the same as if the entry does not exist.
            if (value is null)
            {
                _dictionary.Remove(key);
            }
            else
            {
                _dictionary[key] = value;
            }
        }
    }

    /// <inheritdoc />
    public ICollection<PdfName> Keys
        => _dictionary.Keys;

    /// <inheritdoc />
    public int Count
        => _dictionary.Count;

    /// <summary>
    /// Adds a key/value pair to the dictionary.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="value">The value to add.</param>
    public void Add(PdfName key, IPdfPrimitive value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        if (!_dictionary.TryAdd(key, value))
        {
            throw new PdfException($"{nameof(PdfDictionary)} already contains an entry with key {key.Display}");
        }
    }

    /// <summary>
    /// Removes all key/value pairs from the dictionary.
    /// </summary>
    public void Clear()
        => _dictionary.Clear();

    /// <inheritdoc />
    public bool ContainsKey(PdfName key)
        => _dictionary.ContainsKey(key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<PdfName, IPdfPrimitive>> GetEnumerator()
        => _dictionary.GetEnumerator();

    /// <summary>
    /// Removes the value with the specified key from the dictionary.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if the key was successfully removed; otherwise, false.</returns>
    public bool Remove(PdfName key)
        => _dictionary.Remove(key);

    /// <inheritdoc />
    public bool TryGetValue(PdfName key, [NotNullWhen(true)] out IPdfPrimitive? value)
        => _dictionary.TryGetValue(key, out value);

    /// <inheritdoc />
    public virtual bool TryGetValue<TPrimitive>(PdfName key, [NotNullWhen(true)] out TPrimitive? value)
        where TPrimitive : IPdfPrimitive
    {
        if (_dictionary.TryGetValue(key, out var primitive) && primitive is TPrimitive actualValue)
        {
            value = actualValue;
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => $"Count = {Count}";
}
