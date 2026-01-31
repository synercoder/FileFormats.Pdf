namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Class representing a dash configuration for a stroking action
/// </summary>
public class Dash
{
    /// <summary>
    /// Constructor for <see cref="Dash"/>
    /// </summary>
    public Dash()
    { }

    /// <summary>
    /// Constructor for <see cref="Dash"/>
    /// </summary>
    /// <param name="array">The dash array representing the pattern.</param>
    /// <param name="phase">The phase of the pattern to start in.</param>
    public Dash(double[] array, double phase)
        => (Array, Phase) = (array ?? throw new ArgumentNullException(nameof(array)), phase);

    /// <summary>
    /// Array representing the dash
    /// </summary>
    public IReadOnlyList<double> Array
    {
        get;
        init => field = value ?? throw new ArgumentNullException(nameof(value));
    } = System.Array.Empty<double>();

    /// <summary>
    /// The starting phase of the dash
    /// </summary>
    public double Phase { get; init; } = 0;
}
