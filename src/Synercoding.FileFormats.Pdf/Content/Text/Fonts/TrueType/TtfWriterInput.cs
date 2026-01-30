using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Input data for writing a TrueType font.
/// </summary>
internal sealed class TtfWriterInput
{
    public HeadTable? Head { get; init; }
    public HheaTable? Hhea { get; init; }
    public MaxpTable? Maxp { get; init; }
    public CmapTable? Cmap { get; init; }
    public HmtxTable? Hmtx { get; init; }
    public LocaTable? Loca { get; init; }
    public GlyfTable? Glyf { get; init; }
    public NameTable? Name { get; init; }
    public PostTable? Post { get; init; }
    public OS2Table? OS2 { get; init; }
}
