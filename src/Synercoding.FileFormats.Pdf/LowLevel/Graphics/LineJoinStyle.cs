namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics;

/// <summary>
/// Enum representing the line join style.
/// </summary>
public enum LineJoinStyle
{
    /// <summary>
    /// The outer edges of the strokes for the two segments shall be extended until they meet at an angle.
    /// </summary>
    /// <remarks>
    /// If the segments meet at too sharp an angle (see <see cref="GraphicState.MiterLimit"/>), a bevel join shall be used instead.
    /// </remarks>
    MiterJoin = 0,
    /// <summary>
    /// An arc of a circle with a diameter equal to the line width shall be drawn around the point where the two segments meet,
    /// connecting the outer edges of the strokes for the two segments.
    /// </summary>
    RoundJoin = 1,
    /// <summary>
    /// The two segments shall be finished with <see cref="LineCapStyle.ButtCap"/> and the resulting notch beyond the ends of the segments shall
    /// be filled with a triangle.
    /// </summary>
    BevelJoin = 2
}
