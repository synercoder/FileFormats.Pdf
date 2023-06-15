namespace Synercoding.FileFormats.Pdf.LowLevel.Graphics;

/// <summary>
/// Class representing a dash configuration for a stroking action
/// </summary>
public class Dash
{
    private IReadOnlyList<double> _array = System.Array.Empty<double>();

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
        => (Array, Phase) = (array ?? throw new System.ArgumentNullException(nameof(array)), phase);

    /// <summary>
    /// Array representing the dash
    /// </summary>
    public IReadOnlyList<double> Array
    {
        get => _array;
        init => _array = value ?? throw new System.ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// The starting phase of the dash
    /// </summary>
    public double Phase { get; init; } = 0;
}
