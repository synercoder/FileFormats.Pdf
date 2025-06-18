namespace Synercoding.FileFormats.Pdf.DocumentObjects;

public interface IReadOnlyPage
{
    /// <summary>
    /// A rectangle, expressed in default user space units,
    /// that shall define the boundaries of the physical
    /// medium on which the page shall be displayed or printed.
    /// </summary>
    Rectangle MediaBox { get; }

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the visible region of default user
    ///  space.When the page is displayed or printed, its
    ///  contents shall be clipped (cropped) to this rectangle.
    /// </summary>
    Rectangle? CropBox { get; }

    /// <summary>
    /// A rectangle, expressed in default user space units,
    /// that shall define the region to which the contents
    /// of the page shall be clipped when output in a production
    /// environment.
    /// </summary>
    Rectangle? BleedBox { get; }

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the intended dimensions of the
    ///  finished page after trimming.
    /// </summary>
    Rectangle? TrimBox { get; }

    /// <summary>
    ///  A rectangle, expressed in default user space units,
    ///  that shall define the extent of the page’s meaningful
    ///  content(including potential white-space) as intended
    ///  by the page’s creator.
    /// </summary>
    Rectangle? ArtBox { get; }

    /// <summary>
    ///  A dictionary containing any resources required by the page contents
    /// </summary>
    IReadOnlyResources Resources { get; }

    /// <summary>
    ///  The number of degrees by which the page shall
    ///  be rotated clockwise when displayed or printed.
    ///  The value shall be a multiple of 90.
    /// </summary>
    int? Rotate { get; }

    /// <summary>
    ///  A positive number that shall give the size of default
    ///  user space units, in multiples of 1 ⁄ 72 inch.
    /// </summary>
    double? UserUnit { get; }
}
