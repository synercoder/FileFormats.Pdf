using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    public class PdfStream : IDisposable
    {
        private const string NUMBER_STRING_FORMAT = "0.0########";

        public PdfStream(Stream stream)
            => InnerStream = stream;

        public uint Position
            => (uint)InnerStream.Position;

        protected internal Stream InnerStream { get; }

        public PdfStream WriteByte(byte b)
        {
            InnerStream.WriteByte(b);

            return this;
        }

        public PdfStream Write(char c)
        {
            return WriteByte((byte)(c & 0xFF));
        }

        public PdfStream Write(int value)
        {
            var intSize = ByteSizes.Size(value);
            for (int i = intSize - 1; i >= 0; i--)
            {
                var result = (byte)('0' + ((int)(value / Math.Pow(10, i)) % 10));
                InnerStream.WriteByte(result);
            }

            return this;
        }

        public PdfStream Write(long value)
        {
            var intSize = ByteSizes.Size(value);
            for (int i = intSize - 1; i >= 0; i--)
            {
                var result = (byte)('0' + ((int)(value / Math.Pow(10, i)) % 10));
                InnerStream.WriteByte(result);
            }

            return this;
        }

        public PdfStream Write(double value)
        {
            var stringValue = value.ToString(NUMBER_STRING_FORMAT, CultureInfo.InvariantCulture);
            var bytes = Encoding.ASCII.GetBytes(stringValue);
            InnerStream.Write(bytes, 0, bytes.Length);

            return this;
        }

        public PdfStream Write(string text)
        {
            var bytes = Encoding.ASCII.GetBytes(text);
            InnerStream.Write(bytes, 0, bytes.Length);

            return this;
        }

        public PdfStream Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);

            return this;
        }

        public PdfStream Write(ReadOnlySpan<byte> buffer)
        {
            InnerStream.Write(buffer);

            return this;
        }

        public void Flush()
        {
            InnerStream.Flush();
        }

        public void Dispose()
        {
            InnerStream.Dispose();
        }
    }
}
