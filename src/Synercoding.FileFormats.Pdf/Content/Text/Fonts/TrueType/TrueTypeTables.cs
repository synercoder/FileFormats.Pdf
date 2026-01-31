using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;

/// <summary>
/// Container for TrueType tables
/// </summary>
internal sealed class TrueTypeTables
{
    public HeadTable? Head { get; set; }
    public HheaTable? Hhea { get; set; }
    public MaxpTable? Maxp { get; set; }
    public CmapTable? Cmap { get; set; }
    public HmtxTable? Hmtx { get; set; }
    public NameTable? Name { get; set; }
    public OS2Table? OS2 { get; set; }
    public PostTable? Post { get; set; }
    public LocaTable? Loca { get; set; }
    public GlyfTable? Glyf { get; set; }
}
