namespace Synercoding.FileFormats.Pdf.IO.Filters;

internal static class Predictors
{
    public static byte[] DecodePng(byte[] input, int columns)
    {
        if (input == null || input.Length == 0)
            return Array.Empty<byte>();

        var output = new List<byte>();
        int bytesPerRow = columns;
        int totalRows = input.Length / ( bytesPerRow + 1 );

        var currentRow = new byte[bytesPerRow];
        var previousRow = new byte[bytesPerRow];

        for (int row = 0; row < totalRows; row++)
        {
            int rowStart = row * ( bytesPerRow + 1 );
            byte filterType = input[rowStart];

            // Extract the filtered data for this row
            for (int i = 0; i < bytesPerRow; i++)
                currentRow[i] = input[rowStart + 1 + i];

            // Apply the appropriate PNG filter decoding
            switch (filterType)
            {
                case 0: // None - No prediction
                    // Data is already unfiltered
                    break;

                case 1: // Sub - Predicts the same as the sample to the left
                    for (int i = 1; i < bytesPerRow; i++)
                        currentRow[i] = (byte)( ( currentRow[i] + currentRow[i - 1] ) & 0xFF );
                    break;

                case 2: // Up - Predicts the same as the sample above
                    for (int i = 0; i < bytesPerRow; i++)
                        currentRow[i] = (byte)( ( currentRow[i] + previousRow[i] ) & 0xFF );
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
                        byte upperLeft = i > 0 && row > 0 ? previousRow[i - 1] : (byte)0;

                        int paethValue = _paethPredictor(left, above, upperLeft);
                        currentRow[i] = (byte)( ( currentRow[i] + paethValue ) & 0xFF );
                    }
                    break;

                default:
                    throw new InvalidOperationException($"Unknown PNG filter type: {filterType}");
            }

            output.AddRange(currentRow);

            var tmp = previousRow;
            previousRow = currentRow;
            currentRow = tmp;
        }

        return output.ToArray();
    }

    private static int _paethPredictor(byte left, byte above, byte upperLeft)
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
