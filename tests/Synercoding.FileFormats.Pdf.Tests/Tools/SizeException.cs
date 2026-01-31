namespace Synercoding.FileFormats.Pdf.Tests.Tools;

public class SizeException : Xunit.Sdk.XunitException
{
    public SizeException(object expected, object actual, string methodName)
        : base($"{expected}, {actual}, {methodName} Failure")
    { }
}
