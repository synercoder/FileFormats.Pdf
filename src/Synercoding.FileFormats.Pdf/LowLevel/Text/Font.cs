using System;

namespace Synercoding.FileFormats.Pdf.LowLevel.Text;

/// <summary>
/// Base class representing a font
/// </summary>
public abstract class Font : IEquatable<Font>
{
    /// <inheritdoc />
    public abstract bool Equals(Font? other);
}
