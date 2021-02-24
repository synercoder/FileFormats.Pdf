using System.Collections.Generic;

namespace Synercoding.FileFormats.Pdf.LowLevel.Extensions
{
    public static class PdfStreamArrayExtensions
    {
        private const byte BRACKET_OPEN = 0x5B;  // [
        private const byte BRACKET_CLOSE = 0x5D; // ]

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

        public static PdfStream Write(this PdfStream stream, IEnumerable<PdfReference> objectReferences)
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
