using Synercoding.FileFormats.Pdf.IO;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'OS/2' table in a TrueType font
/// </summary>
internal sealed class OS2Table
{
    /// <summary>
    /// Parse an OS/2 table from bytes
    /// </summary>
    public static OS2Table Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 68)
            throw new InvalidOperationException("OS/2 table too short");

        var table = new OS2Table
        {
            // Version (2 bytes)
            Version = ByteUtils.ReadUInt16BigEndian(data, 0),

            // xAvgCharWidth (2 bytes)
            XAvgCharWidth = ByteUtils.ReadInt16BigEndian(data, 2),

            // usWeightClass (2 bytes)
            WeightClass = ByteUtils.ReadUInt16BigEndian(data, 4),

            // usWidthClass (2 bytes)
            WidthClass = ByteUtils.ReadUInt16BigEndian(data, 6),

            // fsType (2 bytes)
            FsType = ByteUtils.ReadUInt16BigEndian(data, 8),

            // ySubscriptXSize (2 bytes) - skip
            // ySubscriptYSize (2 bytes) - skip
            // ySubscriptXOffset (2 bytes) - skip
            // ySubscriptYOffset (2 bytes) - skip
            // ySuperscriptXSize (2 bytes) - skip
            // ySuperscriptYSize (2 bytes) - skip
            // ySuperscriptXOffset (2 bytes) - skip
            // ySuperscriptYOffset (2 bytes) - skip
            // yStrikeoutSize (2 bytes)
            YStrikeoutSize = ByteUtils.ReadInt16BigEndian(data, 26),

            // yStrikeoutPosition (2 bytes)
            YStrikeoutPosition = ByteUtils.ReadInt16BigEndian(data, 28),

            // sFamilyClass (2 bytes) - skip
            // panose (10 bytes) - skip
            // ulUnicodeRange1-4 (16 bytes) - skip
            // achVendID (4 bytes) - skip
            // fsSelection (2 bytes)
            FsSelection = ByteUtils.ReadUInt16BigEndian(data, 62),

            // usFirstCharIndex (2 bytes)
            FirstCharIndex = ByteUtils.ReadUInt16BigEndian(data, 64),

            // usLastCharIndex (2 bytes)
            LastCharIndex = ByteUtils.ReadUInt16BigEndian(data, 66)
        };

        // Version 0 has more fields
        if (table.Version >= 0 && data.Length >= 78)
        {
            // sTypoAscender (2 bytes)
            table.TypoAscender = ByteUtils.ReadInt16BigEndian(data, 68);

            // sTypoDescender (2 bytes)
            table.TypoDescender = ByteUtils.ReadInt16BigEndian(data, 70);

            // sTypoLineGap (2 bytes)
            table.TypoLineGap = ByteUtils.ReadInt16BigEndian(data, 72);

            // usWinAscent (2 bytes)
            table.WinAscent = ByteUtils.ReadUInt16BigEndian(data, 74);

            // usWinDescent (2 bytes)
            table.WinDescent = ByteUtils.ReadUInt16BigEndian(data, 76);
        }

        // Version 1 has even more fields
        if (table.Version >= 1 && data.Length >= 86)
        {
            // ulCodePageRange1 (4 bytes) - skip
            // ulCodePageRange2 (4 bytes) - skip
        }

        // Version 2+ has additional fields
        if (table.Version >= 2 && data.Length >= 96)
        {
            // sxHeight (2 bytes)
            table.SxHeight = ByteUtils.ReadInt16BigEndian(data, 86);

            // sCapHeight (2 bytes)
            table.SCapHeight = ByteUtils.ReadInt16BigEndian(data, 88);

            // usDefaultChar (2 bytes) - skip
            // usBreakChar (2 bytes) - skip
            // usMaxContext (2 bytes) - skip
        }

        return table;
    }

    /// <summary>
    /// Gets the version
    /// </summary>
    public ushort Version { get; private set; }

    /// <summary>
    /// Gets the average character width
    /// </summary>
    public short XAvgCharWidth { get; private set; }

    /// <summary>
    /// Gets the weight class
    /// </summary>
    public ushort WeightClass { get; private set; }

    /// <summary>
    /// Gets the width class
    /// </summary>
    public ushort WidthClass { get; private set; }

    /// <summary>
    /// Gets the embedding licensing rights
    /// </summary>
    public ushort FsType { get; private set; }

    /// <summary>
    /// Gets the strikeout size
    /// </summary>
    public short YStrikeoutSize { get; private set; }

    /// <summary>
    /// Gets the strikeout position
    /// </summary>
    public short YStrikeoutPosition { get; private set; }

    /// <summary>
    /// Gets the font selection flags
    /// </summary>
    public ushort FsSelection { get; private set; }

    /// <summary>
    /// Gets the first character index
    /// </summary>
    public ushort FirstCharIndex { get; private set; }

    /// <summary>
    /// Gets the last character index
    /// </summary>
    public ushort LastCharIndex { get; private set; }

    /// <summary>
    /// Gets the typographic ascender
    /// </summary>
    public short TypoAscender { get; private set; }

    /// <summary>
    /// Gets the typographic descender
    /// </summary>
    public short TypoDescender { get; private set; }

    /// <summary>
    /// Gets the typographic line gap
    /// </summary>
    public short TypoLineGap { get; private set; }

    /// <summary>
    /// Gets the Windows ascent
    /// </summary>
    public ushort WinAscent { get; private set; }

    /// <summary>
    /// Gets the Windows descent
    /// </summary>
    public ushort WinDescent { get; private set; }

    /// <summary>
    /// Gets the x-height (version 2+)
    /// </summary>
    public short SxHeight { get; private set; }

    /// <summary>
    /// Gets the cap height (version 2+)
    /// </summary>
    public short SCapHeight { get; private set; }
}
