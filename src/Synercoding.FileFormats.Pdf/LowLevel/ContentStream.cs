using SixLabors.ImageSharp;
using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Internal;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Color;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using Synercoding.FileFormats.Pdf.LowLevel.Text;
using Synercoding.Primitives;
using Synercoding.Primitives.Extensions;
using System;
using System.IO;
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

        internal ContentStream(PdfReference id, PageResources pageResources)
        {
            Resources = pageResources;
            InnerStream = new PdfStream(new MemoryStream());

            Reference = id;
        }

        internal PdfStream InnerStream { get; private set; }

        internal PageResources Resources { get; }

        /// <inheritdoc />
        public PdfReference Reference { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            InnerStream.Dispose();
        }

        /// <summary>
        /// Write a save state operator (q) to the stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SaveState()
        {
            InnerStream.Write('q').NewLine();

            return this;
        }

        /// <summary>
        /// Write a restore state operator (Q) to the stream
        /// </summary>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream RestoreState()
        {
            InnerStream.Write('Q').NewLine();

            return this;
        }

        /// <summary>
        /// Write a transformation matrix operator (cm) to the stream
        /// </summary>
        /// <param name="matrix">The <see cref="Matrix"/> to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream CTM(Matrix matrix)
        {
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
                .Write("ET")
                .NewLine();

            return this;
        }

        public ContentStream ShowTextTj(string line)
        {
            InnerStream
                .WriteStringLiteral(line)
                .Space()
                .Write("Tj")
                .NewLine();

            return this;
        }

        public ContentStream MoveNextLineShowText(string line)
        {
            InnerStream
                .WriteStringLiteral(line)
                .Space()
                .Write("'")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the <paramref name="position"/> to the content stream with a Td operator
        /// </summary>
        /// <param name="position">The position to write.</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream SetTextPosition(Point position)
        {
            InnerStream
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
            InnerStream
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
            InnerStream.Write(resource).Space().Write("Do").NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (m) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(MoveOperator op)
        {
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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

            InnerStream
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

            InnerStream
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

            InnerStream
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
                FillRule.NonZeroWindingNumber => InnerStream.Write('f').NewLine(),
                FillRule.EvenOdd => InnerStream.Write('f').Write('*').NewLine(),
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
                FillRule.NonZeroWindingNumber => InnerStream.Write('B').NewLine(),
                FillRule.EvenOdd => InnerStream.Write('B').Write('*').NewLine(),
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
                FillRule.NonZeroWindingNumber => InnerStream.Write('b').NewLine(),
                FillRule.EvenOdd => InnerStream.Write('b').Write('*').NewLine(),
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

            InnerStream
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
                SpotColor spot => Write(new SpotNonStrokingColorOperator(spot)),
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
                SpotColor spot => Write(new SpotStrokingColorOperator(spot)),
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
        /// Write the operator (SCN) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(SpotStrokingColorOperator op)
        {
            var name = Resources.AddSeparation(op.Color.Separation);

            InnerStream
                .Write(name)
                .Space()
                .Write("CS")
                .Space()
                .Write(op.Color.Tint)
                .Space()
                .Write("SCN")
                .NewLine();

            return this;
        }

        /// <summary>
        /// Write the operator (scn) to the stream
        /// </summary>
        /// <param name="op">The operator to write</param>
        /// <returns>The <see cref="ContentStream"/> to support chaining operations.</returns>
        public ContentStream Write(SpotNonStrokingColorOperator op)
        {
            var name = Resources.AddSeparation(op.Color.Separation);

            InnerStream
                .Write(name)
                .Space()
                .Write("cs")
                .Space()
                .Write(op.Color.Tint)
                .Space()
                .Write("scn")
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
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
            InnerStream
                .Write(op.Array)
                .Space()
                .Write(op.Phase)
                .Space()
                .Write('d')
                .NewLine();

            return this;
        }
    }
}
