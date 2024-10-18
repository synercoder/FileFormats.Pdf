namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Class representing an ExtGState dictionary.
/// </summary>
public sealed record class ExtendedGraphicsState
{
    /// <summary>
    /// A flag specifying whether to apply overprint.
    /// There are two separate overprint parameters: one for stroking and one for all other painting operations.
    /// Specifying an <see cref="Overprint"/> entry sets both parameters
    /// unless there is also an <see cref="OverprintNonStroking"/> entry in the same graphics state parameter dictionary,
    /// in which case the <see cref="Overprint"/> entry sets only the overprint parameter for stroking.
    /// </summary>
    public bool? Overprint { get; set; }

    /// <summary>
    /// A flag specifying whether to apply overprint for painting operations other than stroking.
    /// If this entry is absent, the <see cref="Overprint"/> entry, if any, sets this parameter.
    /// </summary>
    public bool? OverprintNonStroking { get; set; }
}

