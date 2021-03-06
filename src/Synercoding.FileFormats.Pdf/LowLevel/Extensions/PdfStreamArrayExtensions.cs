namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="PdfStream"/>
    /// </summary>
    public static class PdfStreamArrayExtensions
    {
        private const byte BRACKET_OPEN = 0x5B;  // [
        private const byte BRACKET_CLOSE = 0x5D; // ]

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="array">The array of doubles to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double[] array)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space();

            foreach (var number in array)
                stream.Write(number).Space();

            stream.WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double number1, double number2)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double number1, double number2, double number3)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double number1, double number2, double number3, double number4)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double number1, double number2, double number3, double number4, double number5)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, double number1, double number2, double number3, double number4, double number5, double number6)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <param name="number7">The seventh number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            double number1, 
            double number2, 
            double number3, 
            double number4, 
            double number5, 
            double number6, 
            double number7)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .Write(number7)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <param name="number7">The seventh number in the array</param>
        /// <param name="number8">The eigth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            double number1, 
            double number2, 
            double number3, 
            double number4, 
            double number5, 
            double number6, 
            double number7, 
            double number8)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .Write(number7)
                .Space()
                .Write(number8)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="array">The array of numbers to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int[] array)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space();

            foreach (var number in array)
                stream.Write(number).Space();

            stream.WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int number1, int number2)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int number1, int number2, int number3)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int number1, int number2, int number3, int number4)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int number1, int number2, int number3, int number4, int number5)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, int number1, int number2, int number3, int number4, int number5, int number6)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <param name="number7">The seventh number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            int number1, 
            int number2, 
            int number3, 
            int number4, 
            int number5, 
            int number6, 
            int number7)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .Write(number7)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of numbers to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="number1">The first number in the array</param>
        /// <param name="number2">The second number in the array</param>
        /// <param name="number3">The third number in the array</param>
        /// <param name="number4">The fourth number in the array</param>
        /// <param name="number5">The fifth number in the array</param>
        /// <param name="number6">The sixth number in the array</param>
        /// <param name="number7">The seventh number in the array</param>
        /// <param name="number8">The eigth number in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            int number1, 
            int number2, 
            int number3, 
            int number4, 
            int number5, 
            int number6, 
            int number7, 
            int number8)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(number1)
                .Space()
                .Write(number2)
                .Space()
                .Write(number3)
                .Space()
                .Write(number4)
                .Space()
                .Write(number5)
                .Space()
                .Write(number6)
                .Space()
                .Write(number7)
                .Space()
                .Write(number8)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="objectReferences">The array of pdfreferences to write</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, PdfReference[] objectReferences)
        {
            stream.WriteByte(BRACKET_OPEN).Space();

            foreach (var objectReference in objectReferences)
            {
                stream.Write(objectReference);
                stream.Space();
            }

            stream.WriteByte(BRACKET_CLOSE).NewLine();

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, PdfReference reference1, PdfReference reference2)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, PdfReference reference1, PdfReference reference2, PdfReference reference3)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <param name="reference4">The fourth reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            PdfReference reference1, 
            PdfReference reference2, 
            PdfReference reference3, 
            PdfReference reference4)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .Write(reference4)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <param name="reference4">The fourth reference in the array</param>
        /// <param name="reference5">The fifth reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            PdfReference reference1, 
            PdfReference reference2, 
            PdfReference reference3,
            PdfReference reference4, 
            PdfReference reference5)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .Write(reference4)
                .Space()
                .Write(reference5)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <param name="reference4">The fourth reference in the array</param>
        /// <param name="reference5">The fifth reference in the array</param>
        /// <param name="reference6">The sixth reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            PdfReference reference1, 
            PdfReference reference2, 
            PdfReference reference3, 
            PdfReference reference4, 
            PdfReference reference5, 
            PdfReference reference6)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .Write(reference4)
                .Space()
                .Write(reference5)
                .Space()
                .Write(reference6)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <param name="reference4">The fourth reference in the array</param>
        /// <param name="reference5">The fifth reference in the array</param>
        /// <param name="reference6">The sixth reference in the array</param>
        /// <param name="reference7">The seventh reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            PdfReference reference1, 
            PdfReference reference2, 
            PdfReference reference3, 
            PdfReference reference4, 
            PdfReference reference5, 
            PdfReference reference6, 
            PdfReference reference7)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .Write(reference4)
                .Space()
                .Write(reference5)
                .Space()
                .Write(reference6)
                .Space()
                .Write(reference7)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }

        /// <summary>
        /// Write an array of pdf references to the pdf stream
        /// </summary>
        /// <param name="stream">The pdf stream to write the array to.</param>
        /// <param name="reference1">The first reference in the array</param>
        /// <param name="reference2">The second reference in the array</param>
        /// <param name="reference3">The third reference in the array</param>
        /// <param name="reference4">The fourth reference in the array</param>
        /// <param name="reference5">The fifth reference in the array</param>
        /// <param name="reference6">The sixth reference in the array</param>
        /// <param name="reference7">The seventh reference in the array</param>
        /// <param name="reference8">The eigth reference in the array</param>
        /// <returns>The <see cref="PdfStream"/> to support chaining operations.</returns>
        public static PdfStream Write(this PdfStream stream, 
            PdfReference reference1, 
            PdfReference reference2, 
            PdfReference reference3, 
            PdfReference reference4, 
            PdfReference reference5, 
            PdfReference reference6, 
            PdfReference reference7, 
            PdfReference reference8)
        {
            stream
                .WriteByte(BRACKET_OPEN)
                .Space()
                .Write(reference1)
                .Space()
                .Write(reference2)
                .Space()
                .Write(reference3)
                .Space()
                .Write(reference4)
                .Space()
                .Write(reference5)
                .Space()
                .Write(reference6)
                .Space()
                .Write(reference7)
                .Space()
                .Write(reference8)
                .Space()
                .WriteByte(BRACKET_CLOSE);

            return stream;
        }
    }
}
