using Synercoding.FileFormats.Pdf.Content;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.Primitives;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Tests.Content;

/// <summary>
/// Regression tests for issue #87 — CID-encoded show-text operands must survive the
/// serialiser unchanged. When literal strings were used, bytes containing 0x0D were
/// normalised to 0x0A by the PDF parser (ISO 32000-1 §7.3.4.2), which silently
/// shifted the CID lookup and produced wrong or missing glyphs.
/// </summary>
public class ContentStreamTests : IDisposable
{
    private readonly TableBuilder _tableBuilder;
    private readonly CachedResources _cachedResources;
    private readonly PageResources _pageResources;
    private readonly ContentStream _contentStream;

    public ContentStreamTests()
    {
        _tableBuilder = new TableBuilder();
        _cachedResources = new CachedResources(_tableBuilder);
        _pageResources = new PageResources(_tableBuilder, _cachedResources);
        _contentStream = new ContentStream(_tableBuilder.ReserveId(), _pageResources);
    }

    public void Dispose()
    {
        _contentStream.Dispose();
        _pageResources.Dispose();
    }

    [Fact]
    public void ShowTextTj_GlyphId0x000D_WritesHexString()
    {
        // Reproduces the original issue #87 case: Source Sans Pro capital 'J'
        // maps to glyph id 13 (0x000D). The bytes must round-trip verbatim.
        _contentStream.ShowTextTj(new byte[] { 0x00, 0x0D });

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<000D>", written);
        Assert.Contains("Tj", written);
    }

    [Fact]
    public void ShowTextTj_GlyphIdWithCarriageReturnInHighByte_WritesHexString()
    {
        _contentStream.ShowTextTj(new byte[] { 0x0D, 0x42 });

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<0D42>", written);
    }

    [Fact]
    public void ShowTextTj_ConsecutiveCidsForming0D0A_PreservesAlignment()
    {
        // A literal string would collapse 0D 0A to a single 0x0A, shifting
        // alignment for every subsequent 2-byte CID from that point on.
        _contentStream.ShowTextTj(new byte[] { 0x01, 0x0D, 0x0A, 0x02 });

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<010D0A02>", written);
    }

    [Fact]
    public void ShowTextTj_BytesMatchingLiteralDelimiters_AreEmittedAsHex()
    {
        // A glyph id whose byte encoding contains '(' / ')' / '\' was previously
        // escaped for a literal string; hex strings write them verbatim.
        _contentStream.ShowTextTj(new byte[] { 0x28, 0x29, 0x5C });

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<28295C>", written);
        Assert.DoesNotContain("\\(", written);
        Assert.DoesNotContain("\\)", written);
        Assert.DoesNotContain("\\\\", written);
    }

    [Fact]
    public void ShowTextTj_DoesNotEmitLiteralStringDelimiters()
    {
        // Regression guard: the operand must no longer be wrapped in ( … ).
        _contentStream.ShowTextTj(new byte[] { 0x00, 0x0D });

        var rawData = _contentStream.InnerStream.ToStreamObject().RawData;

        Assert.DoesNotContain((byte)'(', rawData);
        Assert.DoesNotContain((byte)')', rawData);
    }

    [Fact]
    public void MoveNextLineShowText_GlyphId0x000D_WritesHexString()
    {
        _contentStream.MoveNextLineShowText(new byte[] { 0x00, 0x0D });

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<000D>", written);
        Assert.Contains("'", written);
    }

    [Fact]
    public void MoveNextLineShowText_WithSpacing_WritesHexString()
    {
        _contentStream.MoveNextLineShowText(new byte[] { 0x00, 0x0D }, wordSpacing: 1.0, characterSpacing: 2.0);

        var written = Encoding.ASCII.GetString(_contentStream.InnerStream.ToStreamObject().RawData);

        Assert.Contains("<000D>", written);
        Assert.Contains("\"", written);
    }
}
