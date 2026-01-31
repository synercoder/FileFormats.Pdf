using SixLabors.ImageSharp;

namespace Synercoding.FileFormats.Pdf.Tests.Tools;

public class VisualException : Xunit.Sdk.XunitException
{
    public VisualException(Image expected, Image actual, double difference, string methodName)
        : base($"{expected}, {actual}, {methodName} Failure, actual is {difference * 100}% different")
    {
        Difference = difference;
        ExpectedImage = expected;
        ActualImage = actual;
    }

    public double Difference { get; }

    public Image ExpectedImage { get; }
    public Image ActualImage { get; }
}
