using System.Collections;
using System.Diagnostics;

namespace Synercoding.FileFormats.Pdf.Primitives;

/// <summary>
/// Represents a mutable PDF array that can contain multiple PDF primitive values.
/// </summary>
[DebuggerDisplay("[Pdf Array] {ToString(),nq}")]
public sealed class PdfArray : IPdfArray
{
    private readonly List<IPdfPrimitive> _list;

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfArray"/> class.
    /// </summary>
    public PdfArray()
        : this(Array.Empty<IPdfPrimitive>())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfArray"/> class with integer values.
    /// </summary>
    /// <param name="integers">The integer values to add to the array.</param>
    public PdfArray(int[] integers)
        : this(integers.Select(i => new PdfNumber(i)).Cast<IPdfPrimitive>())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfArray"/> class with double values.
    /// </summary>
    /// <param name="doubles">The double values to add to the array.</param>
    public PdfArray(double[] doubles)
        : this(doubles.Select(d => new PdfNumber(d)).Cast<IPdfPrimitive>())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfArray"/> class with the specified items.
    /// </summary>
    /// <param name="items">The items to add to the array.</param>
    public PdfArray(IEnumerable<IPdfPrimitive> items)
    {
        _list = [.. items];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PdfArray"/> class with the specified items.
    /// </summary>
    /// <param name="items">The items to add to the array.</param>
    public PdfArray(params IPdfPrimitive[] items)
    {
        _list = [.. items];
    }

    /// <inheritdoc />
    public IPdfPrimitive this[int index]
    {
        get { return _list[index]; }
        set { _list[index] = value; }
    }

    /// <inheritdoc />
    public int Count
        => _list.Count;

    /// <summary>
    /// Adds an item to the end of the array.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void Add(IPdfPrimitive item)
        => _list.Add(item);

    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index at which to insert the item.</param>
    /// <param name="item">The item to insert.</param>
    public void Insert(int index, IPdfPrimitive item)
        => _list.Insert(index, item);

    /// <summary>
    /// Removes the first occurrence of a specific item from the array.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was successfully removed; otherwise, false.</returns>
    public bool Remove(IPdfPrimitive item)
        => _list.Remove(item);

    /// <summary>
    /// Removes the item at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public void RemoveAt(int index)
        => _list.RemoveAt(index);

    /// <summary>
    /// Removes all items from the array.
    /// </summary>
    public void Clear()
        => _list.Clear();

    /// <inheritdoc />
    public IEnumerator<IPdfPrimitive> GetEnumerator()
        => _list.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    /// <inheritdoc />
    [DebuggerStepThrough]
    public override string ToString()
        => $"Count = {Count}";
}
