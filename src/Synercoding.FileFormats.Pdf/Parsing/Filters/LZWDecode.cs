using Synercoding.FileFormats.Pdf.Primitives;
using Synercoding.FileFormats.Pdf.Primitives.Extensions;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class LZWDecode : IStreamFilter
{
    public PdfName Name => PdfNames.LZWDecode;

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        if (input.Length == 0)
            return Array.Empty<byte>();

        var earlyChange = parameters != null
            && parameters.TryGetValue<PdfNumber>(PdfNames.EarlyChange, out var earlyChangeNumber)
            && !earlyChangeNumber.IsFractional
            ? earlyChangeNumber.LongValue
            : 1;

        var dictionary = new Dictionary<string, int>();
        var codes = new List<int>();
        
        // Initialize dictionary with single bytes
        for (int i = 0; i < 256; i++)
        {
            dictionary[((char)i).ToString()] = i;
        }
        
        int nextCode = 258; // 256 = clear, 257 = EOD
        string w = string.Empty;
        
        foreach (byte b in input)
        {
            string wc = w + (char)b;
            if (dictionary.ContainsKey(wc))
            {
                w = wc;
            }
            else
            {
                codes.Add(dictionary[w]);
                
                if (nextCode < 4096)
                {
                    dictionary[wc] = nextCode++;
                }
                else
                {
                    // Clear table when full
                    codes.Add(256); // Clear marker
                    dictionary.Clear();
                    for (int i = 0; i < 256; i++)
                    {
                        dictionary[((char)i).ToString()] = i;
                    }
                    nextCode = 258;
                    dictionary[wc] = nextCode++;
                }
                
                w = ((char)b).ToString();
            }
        }
        
        if (!string.IsNullOrEmpty(w))
        {
            codes.Add(dictionary[w]);
        }
        
        codes.Add(257); // EOD marker
        
        return _packCodes(codes, earlyChange);
    }

    public byte[] Decode(byte[] input, IPdfDictionary? parameters, ObjectReader objectReader)
    {
        if (input.Length == 0)
            return Array.Empty<byte>();

        var earlyChange = parameters != null
            && parameters.TryGetValue<PdfNumber>(PdfNames.EarlyChange, objectReader, out var earlyChangeNumber)
            && !earlyChangeNumber.IsFractional
            ? earlyChangeNumber.LongValue
            : 1;

        var codes = _unpackCodes(input, earlyChange);
        if (codes.Length == 0)
            return Array.Empty<byte>();

        var dictionary = new Dictionary<int, byte[]>();
        var result = new List<byte>();
        
        // Initialize dictionary with single bytes
        for (int i = 0; i < 256; i++)
        {
            dictionary[i] = new byte[] { (byte)i };
        }
        
        int nextCode = 258;
        int oldCode = codes[0];
        
        if (oldCode == 257) // EOD at start
            return Array.Empty<byte>();
            
        if (oldCode >= 256)
            throw new InvalidDataException($"Invalid LZW code at start: {oldCode}");
            
        result.Add((byte)oldCode);
        
        for (int i = 1; i < codes.Length; i++)
        {
            int code = codes[i];
            
            if (code == 256) // Clear table
            {
                dictionary.Clear();
                for (int j = 0; j < 256; j++)
                {
                    dictionary[j] = new byte[] { (byte)j };
                }
                nextCode = 258;
                
                if (++i >= codes.Length)
                    break;
                    
                code = codes[i];
                if (code == 257) // EOD after clear
                    break;
                    
                if (code >= 256)
                    throw new InvalidDataException($"Invalid LZW code after clear: {code}");
                    
                result.Add((byte)code);
                oldCode = code;
                continue;
            }
            
            if (code == 257) // EOD
                break;
            
            byte[] entry;
            if (dictionary.ContainsKey(code))
            {
                entry = dictionary[code];
            }
            else if (code == nextCode)
            {
                // Special case: code not in dictionary yet
                var oldEntry = dictionary[oldCode];
                entry = new byte[oldEntry.Length + 1];
                Array.Copy(oldEntry, entry, oldEntry.Length);
                entry[oldEntry.Length] = oldEntry[0];
            }
            else
            {
                throw new InvalidDataException($"Invalid LZW code: {code} (next expected: {nextCode})");
            }
            
            result.AddRange(entry);
            
            // Add new entry to dictionary
            if (nextCode < 4096)
            {
                var oldEntry = dictionary[oldCode];
                var newEntry = new byte[oldEntry.Length + 1];
                Array.Copy(oldEntry, newEntry, oldEntry.Length);
                newEntry[oldEntry.Length] = entry[0];
                dictionary[nextCode++] = newEntry;
            }
            
            oldCode = code;
        }
        
        var decodedData = result.ToArray();
        
        // Apply predictor function if specified
        if (parameters != null && parameters.TryGetValue<PdfNumber>(PdfNames.Predictor, objectReader, out var predictorNumber) && !predictorNumber.IsFractional)
        {
            var predictor = (int)predictorNumber.LongValue;
            if (predictor > 1)
            {
                decodedData = _applyPredictor(decodedData, predictor, parameters, objectReader);
            }
        }
        
        return decodedData;
    }


    private byte[] _packCodes(List<int> codes, long earlyChange = 1)
    {
        var result = new List<byte>();
        int bitsPerCode = 9;
        int bitBuffer = 0;
        int bitCount = 0;
        int nextCodeToAssign = 258;
        
        foreach (int code in codes)
        {
            // Write code with current bit width
            bitBuffer |= (code << bitCount);
            bitCount += bitsPerCode;
            
            // Output complete bytes
            while (bitCount >= 8)
            {
                result.Add((byte)(bitBuffer & 0xFF));
                bitBuffer >>= 8;
                bitCount -= 8;
            }
            
            // Adjust bit width for next code based on EarlyChange parameter
            if (code == 256) // Clear table
            {
                bitsPerCode = 9;
                nextCodeToAssign = 258;
            }
            else if (code == 257) // EOD
            {
                // No change
            }
            else
            {
                // After processing this code, a new dictionary entry will be created
                if (nextCodeToAssign < 4096)
                {
                    nextCodeToAssign++;
                }
                
                // Check if we need to increase bit width based on EarlyChange
                // When EarlyChange=1 (default): increase one code early
                // When EarlyChange=0: postpone as long as possible
                int threshold = earlyChange == 1 ? 0 : 1;
                
                if (nextCodeToAssign >= (512 - threshold) && bitsPerCode == 9)
                    bitsPerCode = 10;
                else if (nextCodeToAssign >= (1024 - threshold) && bitsPerCode == 10)
                    bitsPerCode = 11;
                else if (nextCodeToAssign >= (2048 - threshold) && bitsPerCode == 11)
                    bitsPerCode = 12;
            }
        }
        
        // Flush remaining bits
        if (bitCount > 0)
        {
            result.Add((byte)(bitBuffer & 0xFF));
        }
        
        return result.ToArray();
    }

    private int[] _unpackCodes(byte[] input, long earlyChange = 1)
    {
        var result = new List<int>();
        int bitsPerCode = 9;
        int bitBuffer = 0;
        int bitCount = 0;
        int nextCodeToAssign = 258;
        
        foreach (byte b in input)
        {
            bitBuffer |= (b << bitCount);
            bitCount += 8;
            
            while (bitCount >= bitsPerCode)
            {
                int code = bitBuffer & ((1 << bitsPerCode) - 1);
                result.Add(code);
                
                bitBuffer >>= bitsPerCode;
                bitCount -= bitsPerCode;
                
                if (code == 257) // EOD
                    return result.ToArray();
                    
                if (code == 256) // Clear table
                {
                    bitsPerCode = 9;
                    nextCodeToAssign = 258;
                }
                else
                {
                    // After processing this code, a new dictionary entry will be created
                    // so we need to check if bit width should increase for the next code
                    if (nextCodeToAssign < 4096)
                    {
                        nextCodeToAssign++;
                    }
                    
                    // Increase bit width based on EarlyChange parameter
                    // When EarlyChange=1 (default): increase one code early
                    // When EarlyChange=0: postpone as long as possible
                    int threshold = earlyChange == 1 ? 0 : 1;
                    
                    if (nextCodeToAssign >= (512 - threshold) && bitsPerCode == 9)
                        bitsPerCode = 10;
                    else if (nextCodeToAssign >= (1024 - threshold) && bitsPerCode == 10)
                        bitsPerCode = 11;
                    else if (nextCodeToAssign >= (2048 - threshold) && bitsPerCode == 11)
                        bitsPerCode = 12;
                }
            }
        }
        
        return result.ToArray();
    }

    private byte[] _applyPredictor(byte[] data, int predictor, IPdfDictionary parameters, ObjectReader objectReader)
    {
        var predictors = new Predictors();

        if (predictor == 2)
        {
            // TIFF predictor 2
            var columns = parameters.TryGetValue<PdfNumber>(PdfNames.Columns, objectReader, out var columnsNumber) && !columnsNumber.IsFractional
                ? (int)columnsNumber.LongValue
                : 1;

            var bitsPerComponent = parameters.TryGetValue<PdfNumber>(PdfNames.BitsPerComponent, objectReader, out var bitsNumber) && !bitsNumber.IsFractional
                ? (int)bitsNumber.LongValue
                : 8;

            var colors = parameters.TryGetValue<PdfNumber>(PdfNames.Colors, objectReader, out var colorsNumber) && !colorsNumber.IsFractional
                ? (int)colorsNumber.LongValue
                : 1;

            return predictors.DecodeTiff(data, columns, bitsPerComponent, colors);
        }
        else if (predictor >= 10 && predictor <= 15)
        {
            // PNG predictors
            var columns = parameters.TryGetValue<PdfNumber>(PdfNames.Columns, objectReader, out var columnsNumber) && !columnsNumber.IsFractional
                ? (int)columnsNumber.LongValue
                : 1;

            var bitsPerComponent = parameters.TryGetValue<PdfNumber>(PdfNames.BitsPerComponent, objectReader, out var bitsNumber) && !bitsNumber.IsFractional
                ? (int)bitsNumber.LongValue
                : 8;

            var colors = parameters.TryGetValue<PdfNumber>(PdfNames.Colors, objectReader, out var colorsNumber) && !colorsNumber.IsFractional
                ? (int)colorsNumber.LongValue
                : 1;

            // PNG predictors need the number of bytes per sample (columns * colors * bitsPerComponent / 8)
            var bytesPerRow = columns * colors * ((bitsPerComponent + 7) / 8);
            return predictors.DecodePng(data, bytesPerRow);
        }
        else if (predictor == 1)
        {
            // No prediction
            return data;
        }
        else
        {
            throw new NotSupportedException($"Predictor {predictor} is not supported");
        }
    }
}
