using Synercoding.FileFormats.Pdf.IO;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType.Tables;

/// <summary>
/// Represents the 'name' table in a TrueType font
/// </summary>
internal sealed class NameTable
{
    private readonly Dictionary<int, string> _names = new();

    /// <summary>
    /// Parse a name table from bytes
    /// </summary>
    public static NameTable Parse(ReadOnlySpan<byte> data)
    {
        if (data.Length < 6)
            throw new InvalidOperationException("Name table too short");

        var table = new NameTable();

        // Format (2 bytes)
        var format = ByteUtils.ReadUInt16BigEndian(data, 0);

        // Count (2 bytes)
        var count = ByteUtils.ReadUInt16BigEndian(data, 2);

        // String offset (2 bytes)
        var stringOffset = ByteUtils.ReadUInt16BigEndian(data, 4);

        // Read name records
        var offset = 6;
        for (int i = 0; i < count; i++)
        {
            if (offset + 12 > data.Length)
                break;

            var platformId = ByteUtils.ReadUInt16BigEndian(data, offset);
            var encodingId = ByteUtils.ReadUInt16BigEndian(data, offset + 2);
            var languageId = ByteUtils.ReadUInt16BigEndian(data, offset + 4);
            var nameId = ByteUtils.ReadUInt16BigEndian(data, offset + 6);
            var length = ByteUtils.ReadUInt16BigEndian(data, offset + 8);
            var nameOffset = ByteUtils.ReadUInt16BigEndian(data, offset + 10);

            // We're primarily interested in English names from Windows platform
            if (platformId == 3 && languageId == 0x0409) // Windows, English US
            {
                var strOffset = stringOffset + nameOffset;
                if (strOffset + length <= data.Length)
                {
                    var nameBytes = data.Slice(strOffset, length);
                    var nameStr = Encoding.BigEndianUnicode.GetString(nameBytes);
                    table._names[nameId] = nameStr;
                }
            }
            // Fallback to Mac platform English
            else if (platformId == 1 && languageId == 0 && !table._names.ContainsKey(nameId))
            {
                var strOffset = stringOffset + nameOffset;
                if (strOffset + length <= data.Length)
                {
                    var nameBytes = data.Slice(strOffset, length);
                    var nameStr = Encoding.ASCII.GetString(nameBytes);
                    table._names[nameId] = nameStr;
                }
            }

            offset += 12;
        }

        return table;
    }

    /// <summary>
    /// Gets the PostScript name (nameId 6)
    /// </summary>
    public string? PostScriptName => GetName(6);

    /// <summary>
    /// Gets the full font name (nameId 4)
    /// </summary>
    public string? FullName => GetName(4);

    /// <summary>
    /// Gets the font family name (nameId 1)
    /// </summary>
    public string? FamilyName => GetName(1);

    /// <summary>
    /// Get a name by ID
    /// </summary>
    public string? GetName(int nameId)
    {
        return _names.TryGetValue(nameId, out var name) ? name : null;
    }
}
