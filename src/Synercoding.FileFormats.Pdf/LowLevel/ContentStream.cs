using Synercoding.FileFormats.Pdf.LowLevel.Extensions;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Color;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using System;
using System.IO;

namespace Synercoding.FileFormats.Pdf.LowLevel
{
    public sealed class ContentStream : IPdfObject, IDisposable
    {
        private const string UNKNOWN_FILL_RULE = "Unknown fill rule";
        private readonly PdfStream _streamWrapper;

        public ContentStream(PdfReference id)
        {
            _streamWrapper = new PdfStream(new MemoryStream());
            Reference = id;
        }

        public PdfReference Reference { get; }

        internal bool IsWritten { get; private set; }

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

        public void Dispose()
        {
            _streamWrapper.Dispose();
        }

        public ContentStream SaveState()
        {
            _streamWrapper.Write('q').NewLine();

            return this;
        }

        public ContentStream RestoreState()
        {
            _streamWrapper.Write('Q').NewLine();

            return this;
        }

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

        public ContentStream Paint(PdfName resource)
        {
            _streamWrapper.Write(resource).Space().Write("Do").NewLine();

            return this;
        }

        public ContentStream Write(MoveOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write('m').NewLine();

            return this;
        }

        public ContentStream Write(LineOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write('l').NewLine();

            return this;
        }

        public ContentStream Write(CubicBezierCurveDualControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X1).Space().Write(op.Y1).Space()
                .Write(op.X2).Space().Write(op.Y2).Space()
                .Write(op.X3).Space().Write(op.Y3).Space()
                .Write('c').NewLine();

            return this;
        }

        public ContentStream Write(CubicBezierCurveInitialControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X1).Space().Write(op.Y1).Space()
                .Write(op.X2).Space().Write(op.Y2).Space()
                .Write('v').NewLine();

            return this;
        }

        public ContentStream Write(CubicBezierCurveFinalControlPointsOperator op)
        {
            _streamWrapper
                .Write(op.X1).Space().Write(op.Y1).Space()
                .Write(op.X2).Space().Write(op.Y2).Space()
                .Write('y').NewLine();

            return this;
        }

        public ContentStream Write(RectangleOperator op)
        {
            _streamWrapper
                .Write(op.X).Space().Write(op.Y).Space()
                .Write(op.Width).Space().Write(op.Height).Space()
                .Write("re").NewLine();

            return this;
        }

        public ContentStream Write(CloseOperator op)
        {
            _streamWrapper
                .Write('h').NewLine();

            return this;
        }

        public ContentStream Write(StrokeOperator op)
        {
            _streamWrapper
                .Write('S').NewLine();

            return this;
        }

        public ContentStream Write(CloseAndStrokeOperator op)
        {
            _streamWrapper
                .Write('s').NewLine();

            return this;
        }

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

        public ContentStream Write(EndPathOperator op)
        {
            _streamWrapper
                .Write('n').NewLine();

            return this;
        }

        public ContentStream Write(StrokingColorSpaceOperator op)
        {
            _streamWrapper
                .Write(op.ColorSpace)
                .Space()
                .Write('C').Write('S')
                .NewLine();

            return this;
        }

        public ContentStream Write(NonStrokingColorSpaceOperator op)
        {
            _streamWrapper
                .Write(op.ColorSpace)
                .Space()
                .Write('c').Write('s')
                .NewLine();

            return this;
        }

        public ContentStream Write(GrayStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Gray)
                .Space()
                .Write('G')
                .NewLine();

            return this;
        }

        public ContentStream Write(GrayNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Gray)
                .Space()
                .Write('g')
                .NewLine();

            return this;
        }

        public ContentStream Write(RgbStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Red)
                .Space()
                .Write(op.Green)
                .Space()
                .Write(op.Blue)
                .Space()
                .Write('R')
                .Write('G')
                .NewLine();

            return this;
        }

        public ContentStream Write(RgbNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Red)
                .Space()
                .Write(op.Green)
                .Space()
                .Write(op.Blue)
                .Space()
                .Write('r')
                .Write('g')
                .NewLine();

            return this;
        }

        public ContentStream Write(CmykStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Cyan)
                .Space()
                .Write(op.Magenta)
                .Space()
                .Write(op.Yellow)
                .Space()
                .Write(op.Key)
                .Space()
                .Write('K')
                .NewLine();

            return this;
        }

        public ContentStream Write(CmykNonStrokingColorOperator op)
        {
            _streamWrapper
                .Write(op.Cyan)
                .Space()
                .Write(op.Magenta)
                .Space()
                .Write(op.Yellow)
                .Space()
                .Write(op.Key)
                .Space()
                .Write('k')
                .NewLine();

            return this;
        }

        public ContentStream Write(LineWidthOperator op)
        {
            _streamWrapper
                .Write(op.Width)
                .Space()
                .Write('w')
                .NewLine();

            return this;
        }

        public ContentStream Write(LineCapOperator op)
        {
            _streamWrapper
                .Write((int)op.Style)
                .Space()
                .Write('J')
                .NewLine();

            return this;
        }

        public ContentStream Write(LineJoinOperator op)
        {
            _streamWrapper
                .Write((int)op.Style)
                .Space()
                .Write('j')
                .NewLine();

            return this;
        }

        public ContentStream Write(MiterLimitOperator op)
        {
            _streamWrapper
                .Write(op.Limit)
                .Space()
                .Write('M')
                .NewLine();

            return this;
        }

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
    }
}
