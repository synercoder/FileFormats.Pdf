using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Primitives;
public class PdfNameTests
{
    [Theory]
    [InlineData("test")]
    [InlineData("Data with spaces.")]
    [InlineData("Data with # in it.")]
    [InlineData("こんにちは世界")]
    public void PdfName_Encoding_RoundTrip_Equals_Input(string input)
    {
        var name1 = PdfName.Get(input);
        var name2 = new PdfName(name1.Raw);

        Assert.Equal(input, name2.Display);
    }
}
