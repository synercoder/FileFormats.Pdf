namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Specifies the page layout to be used when the document is opened
/// </summary>
public enum PageLayout
{
    /// <summary>
    /// Display one page at a time
    /// </summary>
    SinglePage,

    /// <summary>
    /// Display the pages in one column
    /// </summary>
    OneColumn,

    /// <summary>
    /// Display the pages in two columns, with odd-numbered pages on the left
    /// </summary>
    TwoColumnLeft,

    /// <summary>
    /// Display the pages in two columns, with odd-numbered pages on the right
    /// </summary>
    TwoColumnRight,

    /// <summary>
    /// Display the pages two at a time, with odd-numbered pages on the left
    /// </summary>
    TwoPageLeft,

    /// <summary>
    /// Display the pages two at a time, with odd-numbered pages on the right
    /// </summary>
    TwoPageRight
}