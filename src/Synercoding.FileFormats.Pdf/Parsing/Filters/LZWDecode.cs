using Synercoding.FileFormats.Pdf.Primitives;

namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

public class LZWDecode : IStreamFilter
{
    public PdfName Name => PdfNames.LZWDecode;

    public byte[] Encode(byte[] input, IPdfDictionary? parameters)
    {
        if (input.Length == 0)
            return Array.Empty<byte>();

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
        
        return _packCodes(codes);
    }

    public byte[] Decode(byte[] input, IPdfDictionary? parameters, ObjectReader objectReader)
    {
        if (input.Length == 0)
            return Array.Empty<byte>();

        var codes = _unpackCodes(input);
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
        
        return result.ToArray();
    }


    private byte[] _packCodes(List<int> codes)
    {
        var result = new List<byte>();
        int bitsPerCode = 9;
        int bitBuffer = 0;
        int bitCount = 0;
        
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
            
            // Adjust bit width for next code based LZW standard
            if (code == 256) // Clear table
            {
                bitsPerCode = 9;
            }
            else if (code == 257) // EOD
            {
                // No change
            }
            else
            {
                // Check if we need to increase bit width
                // Standard LZW increases when the next code to be assigned would require more bits
                int nextCode = result.Count == 1 ? 258 : Math.Max(258, code + 1);
                
                if (nextCode >= 512 && bitsPerCode == 9)
                    bitsPerCode = 10;
                else if (nextCode >= 1024 && bitsPerCode == 10)
                    bitsPerCode = 11;
                else if (nextCode >= 2048 && bitsPerCode == 11)
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

    private int[] _unpackCodes(byte[] input)
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
                    
                    // Increase bit width when we reach the thresholds
                    if (nextCodeToAssign == 512 && bitsPerCode == 9)
                        bitsPerCode = 10;
                    else if (nextCodeToAssign == 1024 && bitsPerCode == 10)
                        bitsPerCode = 11;
                    else if (nextCodeToAssign == 2048 && bitsPerCode == 11)
                        bitsPerCode = 12;
                }
            }
        }
        
        return result.ToArray();
    }
}
