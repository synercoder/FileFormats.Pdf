namespace Synercoding.FileFormats.Pdf.Parsing.Filters;

internal class Predictors
{
    public byte[] DecodePng(byte[] input, int columns)
    {
        if (input == null || input.Length == 0)
            return Array.Empty<byte>();

        var output = new List<byte>();
        int bytesPerRow = columns;
        int totalRows = input.Length / ( bytesPerRow + 1 );

        for (int row = 0; row < totalRows; row++)
        {
            int rowStart = row * ( bytesPerRow + 1 );
            byte filterType = input[rowStart];

            var currentRow = new byte[bytesPerRow];
            var previousRow = row > 0 ? output.Skip(( row - 1 ) * bytesPerRow).Take(bytesPerRow).ToArray() : new byte[bytesPerRow];

            // Extract the filtered data for this row
            for (int i = 0; i < bytesPerRow; i++)
            {
                currentRow[i] = input[rowStart + 1 + i];
            }

            // Apply the appropriate PNG filter decoding
            switch (filterType)
            {
                case 0: // None - No prediction
                    // Data is already unfiltered
                    break;

                case 1: // Sub - Predicts the same as the sample to the left
                    for (int i = 1; i < bytesPerRow; i++)
                    {
                        currentRow[i] = (byte)( ( currentRow[i] + currentRow[i - 1] ) & 0xFF );
                    }
                    break;

                case 2: // Up - Predicts the same as the sample above
                    for (int i = 0; i < bytesPerRow; i++)
                    {
                        currentRow[i] = (byte)( ( currentRow[i] + previousRow[i] ) & 0xFF );
                    }
                    break;

                case 3: // Average - Predicts the average of the sample to the left and the sample above
                    for (int i = 0; i < bytesPerRow; i++)
                    {
                        byte left = i > 0 ? currentRow[i - 1] : (byte)0;
                        byte above = previousRow[i];
                        int average = ( left + above ) / 2;
                        currentRow[i] = (byte)( ( currentRow[i] + average ) & 0xFF );
                    }
                    break;

                case 4: // Paeth - A nonlinear function of the sample above, the sample to the left, and the sample to the upper left
                    for (int i = 0; i < bytesPerRow; i++)
                    {
                        byte left = i > 0 ? currentRow[i - 1] : (byte)0;
                        byte above = previousRow[i];
                        byte upperLeft = ( i > 0 && row > 0 ) ? previousRow[i - 1] : (byte)0;

                        int paethValue = _paethPredictor(left, above, upperLeft);
                        currentRow[i] = (byte)( ( currentRow[i] + paethValue ) & 0xFF );
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown PNG filter type: {filterType}");
            }

            output.AddRange(currentRow);
        }

        return output.ToArray();
    }

    public byte[] DecodeTiff(byte[] input, int columns, int bitsPerComponent, int componentsPerSample)
    {
        if (input == null || input.Length == 0)
            return Array.Empty<byte>();

        // TIFF Predictor 2: predicts that each color component of a sample is the same as 
        // the corresponding color component of the sample immediately to its left

        int bytesPerComponent = ( bitsPerComponent + 7 ) / 8; // Round up to nearest byte
        int bytesPerSample = bytesPerComponent * componentsPerSample;
        int samplesPerRow = columns;
        int bytesPerRow = samplesPerRow * bytesPerSample;

        var output = new byte[input.Length];
        Array.Copy(input, output, input.Length);

        int rows = input.Length / bytesPerRow;

        for (int row = 0; row < rows; row++)
        {
            int rowStart = row * bytesPerRow;

            // Process each sample in the row (skip the first sample as it has no left neighbor)
            for (int sample = 1; sample < samplesPerRow; sample++) 
            {
                int sampleStart = rowStart + (sample * bytesPerSample);
                int leftSampleStart = rowStart + (( sample - 1 ) * bytesPerSample);

                // Process each component of the sample
                for (int component = 0; component < componentsPerSample; component++)
                {
                    int componentStart = sampleStart + (component * bytesPerComponent);
                    int leftComponentStart = leftSampleStart + (component * bytesPerComponent);

                    // Decode based on bits per component
                    if (bitsPerComponent <= 8)
                    {
                        // 8-bit components
                        output[componentStart] = (byte)( ( input[componentStart] + output[leftComponentStart] ) & 0xFF );
                    }
                    else if (bitsPerComponent <= 16)
                    {
                        // 16-bit components (big-endian)
                        int currentValue = ( input[componentStart] << 8 ) | input[componentStart + 1];
                        int leftValue = ( output[leftComponentStart] << 8 ) | output[leftComponentStart + 1];
                        int decodedValue = ( currentValue + leftValue ) & 0xFFFF;

                        output[componentStart] = (byte)( ( decodedValue >> 8 ) & 0xFF );
                        output[componentStart + 1] = (byte)( decodedValue & 0xFF );
                    }
                    else
                    {
                        throw new NotSupportedException($"Bits per component {bitsPerComponent} not supported");
                    }
                }
            }
        }

        return output;
    }

    private int _paethPredictor(byte left, byte above, byte upperLeft)
    {
        int p = left + above - upperLeft;
        int pa = Math.Abs(p - left);
        int pb = Math.Abs(p - above);
        int pc = Math.Abs(p - upperLeft);

        if (pa <= pb && pa <= pc)
            return left;
        else if (pb <= pc)
            return above;
        else
            return upperLeft;
    }
}
