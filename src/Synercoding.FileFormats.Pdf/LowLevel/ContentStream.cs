using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Color;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using System;
using System.IO;

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
