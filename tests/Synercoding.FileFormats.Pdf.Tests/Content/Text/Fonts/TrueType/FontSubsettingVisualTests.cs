using Synercoding.FileFormats.Pdf.Content.Text.Fonts;
using Synercoding.FileFormats.Pdf.Content.Extensions;
using Synercoding.FileFormats.Pdf.Tests.Tools;
using Synercoding.FileFormats.Pdf.IO.Filters;

namespace Synercoding.FileFormats.Pdf.Tests.Content.Text.Fonts.TrueType;

/// <summary>
/// Visual comparison tests for font subsetting to ensure subset and non-subset PDFs render identically.
/// </summary>
public class FontSubsettingVisualTests
{
    private static readonly string _testFontPath = Path.Combine("TestFiles", "Fonts", "JuliettRegular-7OXnA.ttf");

    [Fact]
    public void Test_SubsetFont_BasicText_VisuallyEqual()
    {
        var testText = "The quick brown fox jumps over the lazy dog.";

        var fullFontPdf = _createPdfWithFont(testText, enableSubsetting: false);
        var subsetFontPdf = _createPdfWithFont(testText, enableSubsetting: true);

        using (var fullStream = new MemoryStream(fullFontPdf))
        using (var subsetStream = new MemoryStream(subsetFontPdf))
        {
            PdfAssert.VisualEquals(fullStream, subsetStream, errorTolerance: 0.01, dpi: 300);
        }

        // Verify file size reduction
        Assert.True(subsetFontPdf.Length < fullFontPdf.Length,
            $"Subset PDF should be smaller than full font PDF. Full: {fullFontPdf.Length}, Subset: {subsetFontPdf.Length}");
    }

    [Theory]
    [InlineData("ASCII only text")]
    [InlineData("Numbers: 0123456789")]
    [InlineData("Symbols: .,!@#$%^&*()-=[];'<>?_+")]
    [InlineData("Mixed: Hello123!@#$%^&*()")]
    public void Test_SubsetFont_VariousCharacterSets_VisuallyEqual(string testText)
    {
        var fullFontPdf = _createPdfWithFont(testText, enableSubsetting: false);
        var subsetFontPdf = _createPdfWithFont(testText, enableSubsetting: true);

        using (var fullStream = new MemoryStream(fullFontPdf))
        using (var subsetStream = new MemoryStream(subsetFontPdf))
        {
            PdfAssert.VisualEquals(fullStream, subsetStream, errorTolerance: 0.01, dpi: 300);
        }

        // Verify file size reduction for each character set
        Assert.True(subsetFontPdf.Length < fullFontPdf.Length,
            $"Subset PDF should be smaller for text '{testText}'. Full: {fullFontPdf.Length}, Subset: {subsetFontPdf.Length}");
    }

    [Fact]
    public void Test_SubsetFont_MultiplePages_VisuallyEqual()
    {
        // Test with multiple pages using different subsets of characters
        var page1Text = "Hello World Page 1";
        var page2Text = "Different characters on page 2: 0123456789";

        var fullFontPdf = _createMultiPagePdfWithFont(new[] { page1Text, page2Text }, enableSubsetting: false);
        var subsetFontPdf = _createMultiPagePdfWithFont(new[] { page1Text, page2Text }, enableSubsetting: true);

        using (var fullStream = new MemoryStream(fullFontPdf))
        using (var subsetStream = new MemoryStream(subsetFontPdf))
        {
            PdfAssert.VisualEquals(fullStream, subsetStream, errorTolerance: 0.01, dpi: 300);
        }

        // Verify file size reduction
        Assert.True(subsetFontPdf.Length < fullFontPdf.Length,
            $"Subset PDF should be smaller. Full: {fullFontPdf.Length}, Subset: {subsetFontPdf.Length}");
    }

    [Fact]
    public void Test_SubsetFont_EmptyText_VisuallyEqual()
    {
        // Test edge case with minimal text (should still include .notdef glyph)
        var testText = "A";

        var fullFontPdf = _createPdfWithFont(testText, enableSubsetting: false);
        var subsetFontPdf = _createPdfWithFont(testText, enableSubsetting: true);

        using (var fullStream = new MemoryStream(fullFontPdf))
        using (var subsetStream = new MemoryStream(subsetFontPdf))
        {
            PdfAssert.VisualEquals(fullStream, subsetStream, errorTolerance: 0.01, dpi: 300);
        }

        // Even with minimal text, subset should be smaller
        Assert.True(subsetFontPdf.Length < fullFontPdf.Length,
            $"Subset PDF should be smaller even with minimal text. Full: {fullFontPdf.Length}, Subset: {subsetFontPdf.Length}");
    }

    [Fact]
    public void Test_SubsetFont_RepeatedCharacters_VisuallyEqual()
    {
        // Test with repeated characters to ensure proper deduplication
        var testText = "aaaaaabbbbbbcccccc";

        var fullFontPdf = _createPdfWithFont(testText, enableSubsetting: false);
        var subsetFontPdf = _createPdfWithFont(testText, enableSubsetting: true);

        using (var fullStream = new MemoryStream(fullFontPdf))
        using (var subsetStream = new MemoryStream(subsetFontPdf))
        {
            PdfAssert.VisualEquals(fullStream, subsetStream, errorTolerance: 0.01, dpi: 300);
        }

        Assert.True(subsetFontPdf.Length < fullFontPdf.Length,
            $"Subset PDF should be smaller. Full: {fullFontPdf.Length}, Subset: {subsetFontPdf.Length}");
    }

    /// <summary>
    /// Creates a PDF with the specified text and subsetting setting.
    /// Based on _writeTextOnlyPdf from ConsoleTester.
    /// </summary>
    private static byte[] _createPdfWithFont(string text, bool enableSubsetting)
    {
        using var stream = new MemoryStream();
        using (var writer = new PdfWriter(stream, new WriterSettings
        {
            ContentStreamFilters = Array.Empty<IStreamFilter>(),
            EnableSubsetting = enableSubsetting
        }))
        {
            var font = Font.Load(_testFontPath);

            writer.AddPage(page =>
            {
                page.MediaBox = Sizes.A6.Rotated.AsRectangle();

                // Add the text to the page
                page.Content.AddText(text, font, 18, new Point(Mm(5), Mm(20)));
            });
        }

        return stream.ToArray();
    }

    /// <summary>
    /// Creates a multi-page PDF with the specified texts and subsetting setting.
    /// </summary>
    private static byte[] _createMultiPagePdfWithFont(string[] pageTexts, bool enableSubsetting)
    {
        using var stream = new MemoryStream();
        using (var writer = new PdfWriter(stream, new WriterSettings
        {
            ContentStreamFilters = Array.Empty<IStreamFilter>(),
            EnableSubsetting = enableSubsetting
        }))
        {
            var font = Font.Load(_testFontPath);

            foreach (var text in pageTexts)
            {
                writer.AddPage(page =>
                {
                    page.MediaBox = Sizes.A6.Rotated.AsRectangle();

                    // Add the text to the page
                    page.Content.AddText(text, font, 18, new Point(Mm(5), Mm(20)));
                });
            }
        }

        return stream.ToArray();
    }
}
