using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Color;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Color = Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors.Color;
using Point = Synercoding.Primitives.Point;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    /// <summary>
    /// Class to represent a content stream
    /// </summary>
    public sealed class ContentStream : IPdfObject, IDisposable
    {
        private const string UNKNOWN_FILL_RULE = "Unknown fill rule";
        private readonly PdfStream _streamWrapper;

        /// <summary>
        /// Constructor for <see cref="ContentStream"/>
        /// </summary>
        /// <param name="id">The <see cref="PdfReference"/> of this content stream</param>
        public ContentStream(PdfReference id)
            : this(id, new PdfStream(new MemoryStream()))
        { }

        /// <summary>
        /// Constructor for <see cref="ContentStream"/>
        /// </summary>
        /// <param name="id">The <see cref="PdfReference"/> of this content stream</param>
        /// <param name="pdfStream">The <see cref="PdfStream"/> to write to</param>
        public ContentStream(PdfReference id, PdfStream pdfStream)
        {
            Reference = id;
            _streamWrapper = pdfStream;
        }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        internal bool IsWritten { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            _streamWrapper.Dispose();
        }

        /// <summary>
        /// Write a save state operator (q) to the stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SaveState()
        {
            _streamWrapper.Write('q').NewLine();

            return this;
        }

        /// <summary>
        /// Write a restore state operator (Q) to the stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream RestoreState()
        {
            _streamWrapper.Write('Q').NewLine();

            return this;
        }

        /// <summary>
        /// Write a transformation matrix operator (cm) to the stream
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix"/> to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream CTM(Matrix matrix)
        {
            _streamWrapper
                .Write(matrix.A)
                .Space()
                .Write(matrix.B)
                .Space()
                .Write(matrix.C)
                .Space()
                .Write(matrix.D)
                .Space()
                .Write(matrix.E)
                .Space()
                .Write(matrix.F)
                .Space()
                .Write("cm")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the BT operator to the stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream BeginText()
        {
            _streamWrapper
                .Write("BT")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the font used to the content stream
        /// </summary>
        /// <param name="font">The font to write to the stream</param>
        /// <param name="size">The font size to use</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetFontAndSize(PdfName font, float size)
        {
            _streamWrapper
                .Write(font)
                .Space()
                .Write(size)
                .Space()
                .Write("Tf")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the text leading to the content stream
        /// </summary>
        /// <param name="leading">The leading value to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetTextLeading(float leading)
        {
            _streamWrapper
                .Write(leading)
                .Space()
                .Write("TL")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the character spacing to the content stream
        /// </summary>
        /// <param name="characterSpace">The character spacing value.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetCharacterSpacing(float characterSpace)
        {
            _streamWrapper
                .Write(characterSpace)
                .Space()
                .Write("Tc")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the horizontal scaling to the content stream
        /// </summary>
        /// <param name="horizontalScaling">The horizontal scaling value.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetHorizontalScaling(float horizontalScaling)
        {
            _streamWrapper
                .Write(horizontalScaling)
                .Space()
                .Write("Tz")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the text rise to the content stream
        /// </summary>
        /// <param name="textRise">The text rise value.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetTextRise(float textRise)
        {
            _streamWrapper
                .Write(textRise)
                .Space()
                .Write("Ts")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the text rendering mode to the content stream
        /// </summary>
        /// <param name="textRenderingMode">The text rendering mode value.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetTextRenderMode(TextRenderingMode textRenderingMode)
        {
            _streamWrapper
                .Write((int)textRenderingMode)
                .Space()
                .Write("Tr")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the word spacing to the content stream
        /// </summary>
        /// <param name="wordSpace">The word space value.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetWordSpacing(float wordSpace)
        {
            _streamWrapper
                .Write(wordSpace)
                .Space()
                .Write("Tw")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the end text operator to the content stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream EndText()
        {
            _streamWrapper
                .Write("ET")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write one or more show text operators to the content stream
        /// </summary>
        /// <param name="text">The text to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream ShowText(string text)
        {
            var lines = _splitOnNewLines(text).ToArray();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    _streamWrapper
                        .WriteStringLiteral(lines[i])
                        .Space()
                        .Write("Tj")
                        .NewLine();
                }
                else
                {
                    _streamWrapper
                        .WriteStringLiteral(lines[i])
                        .Space()
                        .Write("'")
                        .NewLine();
                }
            }

            return this;
        }

        private static IEnumerable<string> _splitOnNewLines(string text)
        {
            var builder = new StringBuilder(text.Length);
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var n = i < text.Length - 1
                    ? text[i + 1]
                    : '0';
                if (( c == '\r' && n == '\n' ) || c == '\r' || c == '\n')
                {
                    yield return builder.ToString();
                    builder.Clear();

                    if (n == '\n')
                        i++; // extra skip to also skip the \n
                }
                else
                {
                    builder.Append(c);
                }
            }

            yield return builder.ToString();
        }

        /// <summary>
        /// Write the <paramref name="position"/> to the content stream with a Td operator
        /// </summary>
        /// <param name="position">The position to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetTextPosition(Point position)
        {
            _streamWrapper
                .Write(position.X.AsRaw(Unit.Points))
                .Space()
                .Write(position.Y.AsRaw(Unit.Points))
                .Space()
                .Write("Td")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a text transformation matrix operator (Tm) to the stream
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix"/> to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Tm(Matrix matrix)
        {
            _streamWrapper
                .Write(matrix.A)
                .Space()
                .Write(matrix.B)
                .Space()
                .Write(matrix.C)
                .Space()
                .Write(matrix.D)
                .Space()
                .Write(matrix.E)
                .Space()
                .Write(matrix.F)
                .Space()
                .Write("Tm")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write a xobject paint operator (Do) to the stream
        /// </summary>
        /// <param name="resource">The <see cref="PdfName"/> of the xobject to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Paint(PdfName resource)
        {
            _streamWrapper.Write(resource).Space().Write("Do").NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (m) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(MoveOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write('m').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (l) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(LineOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write('l').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (c) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CubicBezierCurveDualControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X1).Space().Write(op.Y1).Space()
                .Write(op.X2).Space().Write(op.Y2).Space()
                .Write(op.X3).Space().Write(op.Y3).Space()
                .Write('c').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (v) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CubicBezierCurveInitialControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X2).Space().Write(op.Y2).Space()
                .Write(op.X3).Space().Write(op.Y3).Space()
                .Write('v').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (y) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CubicBezierCurveFinalControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X1).Space().Write(op.Y1).Space()
                .Write(op.X3).Space().Write(op.Y3).Space()
                .Write('y').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (re) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(RectangleOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write(op.Width).Space().Write(op.Height).Space()
                .Write("re").NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (h) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CloseOperator op)
        {
            _ = op;

            _streamWrapper
                .Write('h').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (S) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(StrokeOperator op)
        {
            _ = op;

            _streamWrapper
                .Write('S').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (s) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CloseAndStrokeOperator op)
        {
            _ = op;

            _streamWrapper
                .Write('s').NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (f or f*) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(FillOperator op)
        {
            _ = op.FillRule switch
            {
                FillRule.NonZeroWindingNumber => _streamWrapper.Write('f').NewLine(),
                FillRule.EvenOdd => _streamWrapper.Write('f').Write('*').NewLine(),
                _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
            };

            return this;
        }

        /// <summary>
        /// Write the operator (B or B*) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(FillAndStrokeOperator op)
        {
            _ = op.FillRule switch
            {
                FillRule.NonZeroWindingNumber => _streamWrapper.Write('B').NewLine(),
                FillRule.EvenOdd => _streamWrapper.Write('B').Write('*').NewLine(),
                _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
            };

            return this;
        }

        /// <summary>
        /// Write the operator (b or b*) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CloseFillAndStrokeOperator op)
        {
            _ = op.FillRule switch
            {
                FillRule.NonZeroWindingNumber => _streamWrapper.Write('b').NewLine(),
                FillRule.EvenOdd => _streamWrapper.Write('b').Write('*').NewLine(),
                _ => throw new InvalidOperationException(UNKNOWN_FILL_RULE)
            };

            return this;
        }

        /// <summary>
        /// Write the operator (n) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(EndPathOperator op)
        {
            _ = op;

            _streamWrapper
                .Write('n').NewLine();

            return this;
        }

        /// <summary>
        /// Set the color used for filling operations
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        /// <exception cref="NotImplementedException">Will throw if the implementing color type is not supported.</exception>
        public ContentStream SetColorFill(Color color)
        {
            return color switch
            {
                GrayColor gray => Write(new GrayNonStrokingColorOperator(gray)),
                RgbColor rgb => Write(new RgbNonStrokingColorOperator(rgb)),
                CmykColor cmyk => Write(new CmykNonStrokingColorOperator(cmyk)),
                _ => throw new NotImplementedException($"The color type {color.GetType().Name} is not implemented.")
            };
        }

        /// <summary>
        /// Set the color used for stroking operations
        /// </summary>
        /// <param name="color">The color to use</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        /// <exception cref="NotImplementedException">Will throw if the implementing color type is not supported.</exception>
        public ContentStream SetColorStroke(Color color)
        {
            return color switch
            {
                GrayColor gray => Write(new GrayStrokingColorOperator(gray)),
                RgbColor rgb => Write(new RgbStrokingColorOperator(rgb)),
                CmykColor cmyk => Write(new CmykStrokingColorOperator(cmyk)),
                _ => throw new NotImplementedException($"The color type {color.GetType().Name} is not implemented.")
            };
        }

        /// <summary>
        /// Write the operator (S) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(StrokingColorSpaceOperator op)
        {
            _streamWrapper
                .Write(op.ColorSpace)
                .Space()
                .Write('C').Write('S')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (s) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(NonStrokingColorSpaceOperator op)
        {
            _streamWrapper
                .Write(op.ColorSpace)
                .Space()
                .Write('c').Write('s')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (G) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(GrayStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Gray)
                .Space()
                .Write('G')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (g) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(GrayNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Gray)
                .Space()
                .Write('g')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (RG) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(RgbStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Red)
                .Space()
                .Write(op.Color.Green)
                .Space()
                .Write(op.Color.Blue)
                .Space()
                .Write('R')
                .Write('G')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (rg) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(RgbNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Red)
                .Space()
                .Write(op.Color.Green)
                .Space()
                .Write(op.Color.Blue)
                .Space()
                .Write('r')
                .Write('g')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (K) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CmykStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Cyan)
                .Space()
                .Write(op.Color.Magenta)
                .Space()
                .Write(op.Color.Yellow)
                .Space()
                .Write(op.Color.Key)
                .Space()
                .Write('K')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (k) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(CmykNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Color.Cyan)
                .Space()
                .Write(op.Color.Magenta)
                .Space()
                .Write(op.Color.Yellow)
                .Space()
                .Write(op.Color.Key)
                .Space()
                .Write('k')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (w) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(LineWidthOperator op)
        {
            _streamWrapper
                .Write(op.Width)
                .Space()
                .Write('w')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (J) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(LineCapOperator op)
        {
            _streamWrapper
                .Write((int)op.Style)
                .Space()
                .Write('J')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (j) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(LineJoinOperator op)
        {
            _streamWrapper
                .Write((int)op.Style)
                .Space()
                .Write('j')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (M) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(MiterLimitOperator op)
        {
            _streamWrapper
                .Write(op.Limit)
                .Space()
                .Write('M')
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (d) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(DashOperator op)
        {
            _streamWrapper
                .Write(op.Array)
                .Space()
                .Write(op.Phase)
                .Space()
                .Write('d')
                .NewLine();

            return this;
        }

        internal uint WriteToStream(PdfStream stream)
        {
            if (IsWritten)
            {
                throw new InvalidOperationException("Object is already written to stream.");
            }
            var position = (uint)stream.Position;

            _streamWrapper.InnerStream.Position = 0;
            stream.IndirectStream(this, _streamWrapper.InnerStream);
            IsWritten = true;

            return position;
        }
    }
}
