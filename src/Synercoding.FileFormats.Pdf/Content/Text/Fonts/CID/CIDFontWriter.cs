using Synercoding.FileFormats.Pdf.Content.Text.Fonts.TrueType;
using Synercoding.FileFormats.Pdf.Generation;
using Synercoding.FileFormats.Pdf.Generation.Internal;
using Synercoding.FileFormats.Pdf.IO.Filters;
using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Internal;
using System.Text;

namespace Synercoding.FileFormats.Pdf.Content.Text.Fonts.CID;

/// <summary>
/// Writer for CID fonts in PDF
/// </summary>
internal static class CIDFontWriter
{
    /// <summary>
    /// Write a Type 0 font (CID font)
    /// </summary>
    public static void WriteType0Font(ObjectWriter writer, PdfObjectId fontId, Font font, FontUsageTracker tracker, WriterSettings writerSettings)
    {
        if (font is not TrueTypeFont ttFont)
            throw new ArgumentException("Only TrueType fonts are currently supported", nameof(font));

        // Generate IDs for related objects
        var descendantFontId = writer.TableBuiler.ReserveId();
        var fontDescriptorId = writer.TableBuiler.ReserveId();
        var fontFileId = writer.TableBuiler.ReserveId();
        var toUnicodeId = writer.TableBuiler.ReserveId();
        var cidSystemInfoId = writer.TableBuiler.ReserveId();

        // Check if we need to create a CIDToGIDMap
        PdfObjectId? cidToGidMapId = null;
        Dictionary<ushort, ushort>? cidToGidMap = null;

        // Write CIDSystemInfo
        _writeCIDSystemInfo(writer, cidSystemInfoId);

        // Handle font subsetting if enabled and we have tracked usage
        if (writerSettings.EnableSubsetting && tracker.HasUsage)
        {
            var subsetter = new TrueType.FontSubsetter();
            var result = subsetter.CreateSubset(ttFont, tracker);

            // Write subset font file
            _writeFontFileWithSubsetting(writer, fontFileId, result.SubsetFontData, writerSettings);

            // Store CID to GID mapping
            cidToGidMap = result.CidToGidMap;
            if (cidToGidMap != null && !_isIdentityMapping(cidToGidMap))
            {
                cidToGidMapId = writer.TableBuiler.ReserveId();
            }
        }
        else
        {
            // Write full font file
            _writeFontFile(writer, fontFileId, ttFont, writerSettings);
        }

        // Write font descriptor
        _writeFontDescriptor(writer, fontDescriptorId, ttFont, fontFileId);

        // Write CID font (descendant font) with optional CIDToGIDMap
        _writeCIDFont(writer, descendantFontId, ttFont, fontDescriptorId, cidSystemInfoId, tracker, cidToGidMapId);

        // Write CIDToGIDMap if needed
        if (cidToGidMapId != null && cidToGidMap != null)
        {
            _writeCIDToGIDMap(writer, cidToGidMapId.Value, cidToGidMap, writerSettings);
        }

        // Write ToUnicode CMap
        _writeToUnicodeCMap(writer, toUnicodeId, ttFont, tracker, writerSettings);

        // Write Type 0 font (main font dictionary)
        var fontDict = new PdfDictionary
        {
            [PdfNames.Type] = PdfNames.Font,
            [PdfNames.Subtype] = PdfNames.Type0,
            [PdfNames.BaseFont] = PdfName.Get(ttFont.FontName),
            [PdfNames.Encoding] = PdfNames.IdentityH,
            [PdfNames.DescendantFonts] = new PdfArray() { descendantFontId.GetReference() },
            [PdfNames.ToUnicode] = toUnicodeId.GetReference()
        };

        writer.Write(new PdfObject<PdfDictionary>()
        {
            Id = fontId,
            Value = fontDict
        });
    }

    private static void _writeCIDSystemInfo(ObjectWriter writer, PdfObjectId cidSystemInfoId)
    {
        var cidSystemInfo = new PdfDictionary
        {
            [PdfNames.Registry] = new PdfString(Encoding.ASCII.GetBytes("Adobe"), false),
            [PdfNames.Ordering] = new PdfString(Encoding.ASCII.GetBytes("Identity"), false),
            [PdfNames.Supplement] = new PdfNumber(0)
        };

        writer.Write(new PdfObject<PdfDictionary>()
        {
            Id = cidSystemInfoId,
            Value = cidSystemInfo
        });
    }

    private static void _writeFontFile(ObjectWriter writer, PdfObjectId fontFileId, TrueTypeFont font, WriterSettings writerSettings)
    {
        var fontData = font.GetFontData();

        // Compress font data
        var flate = new FlateDecode();
        byte[] compressedData = flate.Encode(fontData, null);

        var streamDict = new PdfDictionary
        {
            [PdfNames.Length1] = new PdfNumber(fontData.Length),
            [PdfNames.Filter] = flate.Name,
            [PdfNames.Length] = new PdfNumber(compressedData.Length)
        };

        var fontStream = new ReadOnlyPdfStreamObject(streamDict, compressedData);

        writer.Write(new PdfObject<IPdfStreamObject>()
        {
            Id = fontFileId,
            Value = fontStream
        });
    }

    private static void _writeFontFileWithSubsetting(ObjectWriter writer, PdfObjectId fontFileId, byte[] fontData, WriterSettings writerSettings)
    {
        // Compress font data
        var flate = new FlateDecode();
        byte[] compressedData = flate.Encode(fontData, null);

        var streamDict = new PdfDictionary
        {
            [PdfNames.Length1] = new PdfNumber(fontData.Length),
            [PdfNames.Filter] = flate.Name,
            [PdfNames.Length] = new PdfNumber(compressedData.Length)
        };

        var fontStream = new ReadOnlyPdfStreamObject(streamDict, compressedData);

        writer.Write(new PdfObject<IPdfStreamObject>()
        {
            Id = fontFileId,
            Value = fontStream
        });
    }

    private static void _writeFontDescriptor(ObjectWriter writer, PdfObjectId descriptorId, TrueTypeFont font, PdfObjectId fontFileId)
    {
        var tables = font.Tables;
        var head = tables.Head!;
        var hhea = tables.Hhea!;
        var post = tables.Post;
        var os2 = tables.OS2;

        // Calculate font flags
        int flags = 32; // Nonsymbolic (bit 6)
        if (post?.IsFixedPitch == true)
            flags |= 1; // FixedPitch (bit 1)
        if (post?.ItalicAngle != 0)
            flags |= 64; // Italic (bit 7)

        var descriptor = new PdfDictionary()
        {
            [PdfNames.Type] = PdfNames.FontDescriptor,
            [PdfNames.FontName] = PdfName.Get(font.FontName),
            [PdfNames.Flags] = new PdfNumber(flags),
            [PdfNames.FontBBox] = new PdfArray
            {
                new PdfNumber(_scaleToThousandths(head.XMin, head.UnitsPerEm)),
                new PdfNumber(_scaleToThousandths(head.YMin, head.UnitsPerEm)),
                new PdfNumber(_scaleToThousandths(head.XMax, head.UnitsPerEm)),
                new PdfNumber(_scaleToThousandths(head.YMax, head.UnitsPerEm))
            },
            [PdfNames.ItalicAngle] = new PdfNumber(post?.ItalicAngle ?? 0),
            [PdfNames.Ascent] = new PdfNumber(_scaleToThousandths(os2?.TypoAscender ?? hhea.Ascender, head.UnitsPerEm)),
            [PdfNames.Descent] = new PdfNumber(_scaleToThousandths(os2?.TypoDescender ?? hhea.Descender, head.UnitsPerEm)),
            [PdfNames.CapHeight] = new PdfNumber(_scaleToThousandths(os2?.SCapHeight ?? (int)( hhea.Ascender * 0.7 ), head.UnitsPerEm)),
            [PdfNames.StemV] = new PdfNumber(80), // Default value
            [PdfNames.FontFile2] = fontFileId.GetReference()
        };

        writer.Write(new PdfObject<PdfDictionary>
        {
            Id = descriptorId,
            Value = descriptor
        });
    }

    private static void _writeCIDFont(ObjectWriter writer, PdfObjectId cidFontId, TrueTypeFont font,
        PdfObjectId fontDescriptorId, PdfObjectId cidSystemInfoId, FontUsageTracker tracker, PdfObjectId? cidToGidMapId = null)
    {
        var cidFont = new PdfDictionary()
        {
            [PdfNames.Type] = PdfNames.Font,
            [PdfNames.Subtype] = PdfNames.CIDFontType2,
            [PdfNames.BaseFont] = PdfName.Get(font.FontName),
            [PdfNames.CIDSystemInfo] = cidSystemInfoId.GetReference(),
            [PdfNames.FontDescriptor] = fontDescriptorId.GetReference(),
            [PdfNames.DW] = new PdfNumber(1000) // Default width
        };

        // Add width array if we have usage tracking
        var widthArray = _createWidthArray(font, tracker);
        if (widthArray.Count > 0)
        {
            cidFont[PdfNames.W] = widthArray;
        }

        // Add CIDToGIDMap if provided
        if (cidToGidMapId != null)
        {
            cidFont[PdfNames.CIDToGIDMap] = cidToGidMapId.Value.GetReference();
        }
        else
        {
            // Use /Identity for non-subset fonts
            cidFont[PdfNames.CIDToGIDMap] = PdfNames.Identity;
        }

        writer.Write(new PdfObject<PdfDictionary>()
        {
            Id = cidFontId,
            Value = cidFont
        });
    }

    private static PdfArray _createWidthArray(TrueTypeFont font, FontUsageTracker tracker)
    {
        var tables = font.Tables;
        var head = tables.Head!;
        var hmtx = tables.Hmtx!;
        var cmap = tables.Cmap!;

        var widthArray = new PdfArray();

        // Build width entries for used characters
        var widthEntries = new List<(int cid, int width)>();

        if (tracker.HasUsage)
        {
            foreach (var ch in tracker.UsedCharacters.OrderBy(c => c))
            {
                var glyphId = cmap.GetGlyphId(ch);
                var width = hmtx.GetAdvanceWidth(glyphId);
                var scaledWidth = _scaleToThousandths(width, head.UnitsPerEm);
                widthEntries.Add((glyphId, scaledWidth));
            }
        }
        else
        {
            // If no usage tracked, include basic ASCII range
            for (int i = 32; i < 127; i++)
            {
                var glyphId = cmap.GetGlyphId((char)i);
                var width = hmtx.GetAdvanceWidth(glyphId);
                var scaledWidth = _scaleToThousandths(width, head.UnitsPerEm);
                widthEntries.Add((glyphId, scaledWidth));
            }
        }

        if (widthEntries.Count == 0)
            return widthArray;

        // Group consecutive CIDs with same width
        int startCid = widthEntries[0].cid;
        int endCid = startCid;
        int currentWidth = widthEntries[0].width;
        var rangeWidths = new List<int> { currentWidth };

        for (int i = 1; i < widthEntries.Count; i++)
        {
            var (cid, width) = widthEntries[i];

            if (cid == endCid + 1)
            {
                // Consecutive CID
                endCid = cid;
                rangeWidths.Add(width);
            }
            else
            {
                // Gap in CIDs, write current range
                _addWidthRange(widthArray, startCid, rangeWidths);

                // Start new range
                startCid = cid;
                endCid = cid;
                rangeWidths.Clear();
                rangeWidths.Add(width);
            }
        }

        // Write last range
        if (rangeWidths.Count > 0)
        {
            _addWidthRange(widthArray, startCid, rangeWidths);
        }

        return widthArray;
    }

    private static void _addWidthRange(PdfArray widthArray, int startCid, List<int> widths)
    {
        if (widths.Count == 1 || widths.All(w => w == widths[0]))
        {
            // All same width: use format "c_first c_last w"
            widthArray.Add(new PdfNumber(startCid));
            widthArray.Add(new PdfNumber(startCid + widths.Count - 1));
            widthArray.Add(new PdfNumber(widths[0]));
        }
        else
        {
            // Different widths: use format "c [w1 w2 ...]"
            widthArray.Add(new PdfNumber(startCid));
            var widthSubArray = new PdfArray();
            foreach (var width in widths)
            {
                widthSubArray.Add(new PdfNumber(width));
            }
            widthArray.Add(widthSubArray);
        }
    }

    private static void _writeToUnicodeCMap(ObjectWriter writer, PdfObjectId cmapId, TrueTypeFont font, FontUsageTracker tracker, WriterSettings writerSettings)
    {
        var cmap = _generateToUnicodeCMap(font, tracker);
        var cmapBytes = Encoding.ASCII.GetBytes(cmap);

        // Compress the CMap
        var flate = new FlateDecode();
        byte[] compressedData = flate.Encode(cmapBytes, null);

        var streamDict = new PdfDictionary()
        {
            [PdfNames.Filter] = flate.Name,
            [PdfNames.Length] = new PdfNumber(compressedData.Length)
        };

        var cmapStream = new ReadOnlyPdfStreamObject(streamDict, compressedData);

        writer.Write(new PdfObject<IPdfStreamObject>()
        {
            Id = cmapId,
            Value = cmapStream
        });
    }

    private static string _generateToUnicodeCMap(TrueTypeFont font, FontUsageTracker tracker)
    {
        var sb = new StringBuilder();

        // CMap header
        sb.AppendLine("/CIDInit /ProcSet findresource begin");
        sb.AppendLine("12 dict begin");
        sb.AppendLine("begincmap");
        sb.AppendLine("/CIDSystemInfo");
        sb.AppendLine("<< /Registry (Adobe)");
        sb.AppendLine("/Ordering (UCS)");
        sb.AppendLine("/Supplement 0");
        sb.AppendLine(">> def");
        sb.AppendLine("/CMapName /Adobe-Identity-UCS def");
        sb.AppendLine("/CMapType 2 def");
        sb.AppendLine("1 begincodespacerange");
        sb.AppendLine("<0000> <FFFF>");
        sb.AppendLine("endcodespacerange");

        // Build mappings
        var mappings = new List<(int cid, int unicode)>();

        if (tracker.HasUsage)
        {
            foreach (var ch in tracker.UsedCharacters)
            {
                mappings.Add((font.GetGlyphId(ch), ch));
            }
        }
        else
        {
            // Basic ASCII range
            for (int i = 32; i < 127; i++)
            {
                mappings.Add((font.GetGlyphId((char)i), i));
            }
        }

        if (mappings.Count > 0)
        {
            // Sort by CID
            mappings.Sort((a, b) => a.cid.CompareTo(b.cid));

            // Write mappings in chunks of 100
            const int chunkSize = 100;
            for (int i = 0; i < mappings.Count; i += chunkSize)
            {
                var chunk = mappings.Skip(i).Take(Math.Min(chunkSize, mappings.Count - i)).ToList();
                sb.AppendLine($"{chunk.Count} beginbfchar");

                foreach (var (cid, unicode) in chunk)
                {
                    sb.AppendLine($"<{cid:X4}> <{unicode:X4}>");
                }

                sb.AppendLine("endbfchar");
            }
        }

        // CMap footer
        sb.AppendLine("endcmap");
        sb.AppendLine("CMapName currentdict /CMap defineresource pop");
        sb.AppendLine("end");
        sb.AppendLine("end");

        return sb.ToString();
    }


    private static int _scaleToThousandths(int value, int unitsPerEm)
    {
        // Scale value from font units to PDF's 1/1000 units
        return (int)Math.Round(value * 1000.0 / unitsPerEm);
    }

    private static bool _isIdentityMapping(Dictionary<ushort, ushort> cidToGid)
    {
        // Check if this is an identity mapping (CID == GID for all entries)
        foreach (var kvp in cidToGid)
        {
            if (kvp.Key != kvp.Value)
                return false;
        }
        return true;
    }

    private static void _writeCIDToGIDMap(ObjectWriter writer, PdfObjectId mapId, Dictionary<ushort, ushort> cidToGid, WriterSettings writerSettings)
    {
        // Create a stream with 2-byte entries mapping CID to GID
        // The stream contains GID values indexed by CID

        // Find the maximum CID to determine stream size
        ushort maxCid = cidToGid.Keys.DefaultIfEmpty((ushort)0).Max();

        // Create byte array with 2 bytes per entry (up to maxCid + 1)
        var mapData = new byte[( maxCid + 1 ) * 2];

        // Initialize all entries to 0 (maps to .notdef)
        // This is already the case since byte array is zero-initialized

        // Fill in the actual mappings
        foreach (var kvp in cidToGid)
        {
            var cid = kvp.Key;
            var gid = kvp.Value;

            // Write GID in big-endian format at position CID * 2
            mapData[cid * 2] = (byte)( gid >> 8 );
            mapData[( cid * 2 ) + 1] = (byte)gid;
        }

        // Compress the map data
        var flate = new FlateDecode();
        byte[] compressedData = flate.Encode(mapData, null);

        var streamDict = new PdfDictionary
        {
            [PdfNames.Filter] = flate.Name,
            [PdfNames.Length] = new PdfNumber(compressedData.Length)
        };

        var mapStream = new ReadOnlyPdfStreamObject(streamDict, compressedData);

        writer.Write(new PdfObject<IPdfStreamObject>()
        {
            Id = mapId,
            Value = mapStream
        });
    }
}
