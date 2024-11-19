using Synercoding.FileFormats.Pdf.Exceptions;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;
public class TrailerTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(123)]
    [InlineData(int.MaxValue)]
    public void Test_If_Size_Is_Read_Correctly(int size)
    {
        var dictionary = new PdfDictionary
        {
            [PdfNames.Size] = new PdfInteger(size)
        };

        var trailer = new Trailer(dictionary);

        Assert.Equal(size, trailer.Size);
    }

    [Fact]
    public void Test_If_Throws_With_Negative_Size()
    {
        const int SIZE = -1;
        var dictionary = new PdfDictionary
        {
            [PdfNames.Size] = new PdfInteger(SIZE)
        };

        var trailer = new Trailer(dictionary);

        Assert.Throws<ParseException>(() => trailer.Size);
    }
}
