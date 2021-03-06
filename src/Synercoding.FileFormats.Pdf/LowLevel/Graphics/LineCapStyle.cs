namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics
{
    /// <summary>
    /// Enum representing the shape that shall be used at the ends of open subpaths (and dashes, if any) when they are stroked.
    /// </summary>
    public enum LineCapStyle
    {
        /// <summary>
        /// The stroke shall be squared off at the endpoint of the path.
        /// </summary>
        ButtCap = 0,
        /// <summary>
        /// A semicircular arc with a diameter equal to the line width shall be drawn around the endpoint and shall be filled in.
        /// </summary>
        RoundCap = 1,
        /// <summary>
        /// The stroke shall continue beyond the endpoint of the path for a distance equal to half the line width and shall be squared off.
        /// </summary>
        ProjectingSquareCap = 2
    }
}
