namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Specifies how the document should be displayed when opened
/// </summary>
public enum PageMode
{
    /// <summary>
    /// Neither document outline nor thumbnail images visible
    /// </summary>
    UseNone,

    /// <summary>
    /// Document outline visible
    /// </summary>
    UseOutlines,

    /// <summary>
    /// Thumbnail images visible
    /// </summary>
    UseThumbs,

    /// <summary>
    /// Full-screen mode, with no menu bar, window controls, or any other window visible
    /// </summary>
    FullScreen,

    /// <summary>
    /// Optional content group panel visible
    /// </summary>
    UseOC,

    /// <summary>
    /// Attachments panel visible
    /// </summary>
    UseAttachments
}