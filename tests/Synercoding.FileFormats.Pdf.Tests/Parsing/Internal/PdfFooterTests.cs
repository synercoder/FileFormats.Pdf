using Synercoding.FileFormats.Pdf.IO;
using Synercoding.FileFormats.Pdf.Parsing.Internal;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Parsing.Internal;

public class PdfFooterTests
{

    [Fact]
    public void Test_Can_Parse()
    {
        var reader = new ObjectReader(Array.Empty<byte>());

        var provider = new PdfByteArrayProvider(Encoding.ASCII.GetBytes(TestCases.FOOTER_WITH_1_XREF_ITEM));
        var (trailer, table) = PdfFooter.Parse(provider, 0, reader);

        Assert.Equal(1, trailer.Size);
        Assert.Single(table.Items);
    }

    [Fact]
    public void Test_Can_MergeIncrementalUpdates()
    {
        var reader = new ObjectReader(Array.Empty<byte>());

        var provider = new PdfByteArrayProvider(Encoding.ASCII.GetBytes(TestCases.FILE_WITH_INCREMENTAL_UPDATE));
        var (trailer, table) = PdfFooter.Parse(provider, 0, reader);

        Assert.Equal(4, trailer.Size);
        Assert.Equal(4, table.Items.Length);
    }

    private static class TestCases
    {
        public const string FOOTER_WITH_1_XREF_ITEM = "xref\r\n"
            + "0 1\r\n"
            + "0000000000 65535 f\r\n"
            + "trailer\r\n"
            + "<<\r\n"
            + "/Size 1\r\n"
            + ">>\r\n"
            + "startxref\r\n"
            + "0\r\n"
            + "%%EOF";

        public const string FILE_WITH_INCREMENTAL_UPDATE = "%PDF-1.7\r\n"
            + "%   \r\n"
            + "1 0 obj\r\n"
            + "null\r\n"
            + "endobj\r\n"
            + "2 0 obj\r\n"
            + "null\r\n"
            + "endobj\r\n"
            + "xref\r\n"
            + "0 3\r\n"
            + "0000000000 65535 f\r\n"
            + "0000000016 0 n\r\n"
            + "0000000039 0 n\r\n"
            + "trailer\r\n"
            + "<<\r\n"
            + "/Size 3\r\n"
            + ">>\r\n"
            + "startxref\r\n"
            + "62\r\n"
            + "%%EOF\r\n"
            + "3 0 obj\r\n"
            + "null\r\n"
            + "endobj\r\n"
            + "xref\r\n"
            + "4 1\r\n"
            + "0000000173 0 n\r\n"
            + "trailer\r\n"
            + "<<\r\n"
            + "/Size 4\r\n"
            + "/Prev 62\r\n"
            + ">>\r\n"
            + "startxref\r\n"
            + "196\r\n"
            + "%%EOF";
    }

}
