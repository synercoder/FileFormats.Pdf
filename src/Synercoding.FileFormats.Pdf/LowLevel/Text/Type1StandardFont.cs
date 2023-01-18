using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Text;

/// <summary>
/// Class representing a type1 standard font
/// </summary>
public sealed class Type1StandardFont : Font, IEquatable<Type1StandardFont>
{
    internal Type1StandardFont(PdfName name, PdfName lookupName)
    {
        Name = name;
        LookupName = lookupName;
    }

    /// <summary>
    /// The name of the font
    /// </summary>
    public PdfName Name { get; }

    internal PdfName LookupName { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Type1StandardFont font && Equals(font);

    /// <inheritdoc />
    public bool Equals(Type1StandardFont? other)
        => other?.GetHashCode() == GetHashCode();

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(Name, LookupName);
}
