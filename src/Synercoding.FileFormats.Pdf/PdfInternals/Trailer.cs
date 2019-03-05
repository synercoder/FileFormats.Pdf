using Synercoding.FileFormats.Pdf.Helpers;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.PdfInternals
{
    internal struct Trailer : ISpanWriteable, IStreamWriteable
    {
        public Trailer(uint startXRef, int size, int root)
        {
            StartXRef = startXRef;
            Size = size;
            Root = root;
        }

        public uint StartXRef { get; }
        public int Size { get; }
        public int Root { get; }

        public void FillSpan(Span<byte> bytes)
        {
            var position = 0;
            _writeTrailer(ref bytes, ref position);

            _writeDictStart(ref bytes, ref position);

            _writeSize(ref bytes, ref position);
            _writeRoot(ref bytes, ref position);

            _writeDictClose(ref bytes, ref position);

            _writeStartXRef(ref bytes, ref position);

            _writeEndOfFile(ref bytes, ref position);
        }

        private void _writeTrailer(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x74; // t
            bytes[position++] = 0x72; // r
            bytes[position++] = 0x61; // a
            bytes[position++] = 0x69; // i
            bytes[position++] = 0x6C; // l
            bytes[position++] = 0x65; // e
            bytes[position++] = 0x72; // r
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        private static void _writeEndOfFile(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x25; // %
            bytes[position++] = 0x25; // %
            bytes[position++] = 0x45; // E
            bytes[position++] = 0x4F; // O
            bytes[position++] = 0x46; // F
        }

        private void _writeSize(ref Span<byte> bytes, ref int position)
        {
            _writeSizeKey(ref bytes, ref position);
            bytes[position++] = 0x20; // space
            position += SpanHelper.FillSpan(bytes.Slice(position), Size);
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        private void _writeSizeKey(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x2F; // /
            bytes[position++] = 0x53; // S
            bytes[position++] = 0x69; // i
            bytes[position++] = 0x7A; // z
            bytes[position++] = 0x65; // e
        }

        private void _writeRoot(ref Span<byte> bytes, ref int position)
        {
            _writeRootKey(ref bytes, ref position);
            bytes[position++] = 0x20; // space
            position += SpanHelper.WriteIndirectObjectRef(bytes.Slice(position), Root, 0);
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        private void _writeRootKey(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x2F; // /
            bytes[position++] = 0x52; // R
            bytes[position++] = 0x6F; // o
            bytes[position++] = 0x6F; // o
            bytes[position++] = 0x74; // t
        }

        private void _writeDictStart(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x3C; // <
            bytes[position++] = 0x3C; // <
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        private void _writeDictClose(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x3E; // >
            bytes[position++] = 0x3E; // >
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        private void _writeStartXRef(ref Span<byte> bytes, ref int position)
        {
            bytes[position++] = 0x73; // s
            bytes[position++] = 0x74; // t
            bytes[position++] = 0x61; // a
            bytes[position++] = 0x72; // r
            bytes[position++] = 0x74; // t
            bytes[position++] = 0x78; // x
            bytes[position++] = 0x72; // r
            bytes[position++] = 0x65; // e
            bytes[position++] = 0x66; // f
            position += SpanHelper.WriteNewLine(bytes.Slice(position));

            position += SpanHelper.FillSpan(bytes.Slice(position), StartXRef);
            position += SpanHelper.WriteNewLine(bytes.Slice(position));
        }

        public int ByteSize()
        {
            int size = 7; // trailer
            size += 2; // CR LF
            size += 4; // dict start
            size += 5; // size key
            size += 1; // space
            size += ByteSizes.Size(Size);
            size += 2; // CR LF
            size += 5; // root key
            size += 1; // space
            size += ByteSizes.Size(Root);
            size += 1; // space
            size += 1; // gen 0
            size += 1; // space
            size += 1; // R
            size += 2; // CR LF
            size += 4; // dict end
            size += 9; // startxref
            size += 2; // CR LF
            size += ByteSizes.Size(StartXRef);
            size += 2; // CR LF
            size += 5; // %%EOF

            return size;
        }

        public uint WriteToStream(Stream stream)
        {
            var position = (uint)stream.Position;

            var bytes = new byte[ByteSize()];
            FillSpan(bytes);
            stream.Write(bytes, 0, bytes.Length);

            return position;
        }
    }
}