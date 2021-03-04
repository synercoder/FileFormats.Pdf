using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Color;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Construction;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.Pathing.Painting;
using Synercoding.FileFormats.Pdf.LowLevel.Operators.State;
using Synercoding.Primitives;
using System;

namespace Synercoding.FileFormats.Pdf.Internals
{
    internal class Path : IPath
    {
        private readonly ShapeContext _context;
        private readonly ContentStream _contentStream;

        public Path(ShapeContext context, ContentStream contentStream, GraphicsState graphicsState)
        {
            _context = context;
            _contentStream = contentStream;
            GraphicsState = graphicsState;

            _startPath();
        }

        internal GraphicsState GraphicsState { get; }

        private void _startPath()
        {
            _contentStream.SaveState();

            _contentStream
                .Write(new LineWidthOperator(GraphicsState.LineWidth))
                .Write(new LineCapOperator(GraphicsState.LineCap))
                .Write(new LineJoinOperator(GraphicsState.LineJoin))
                .Write(new MiterLimitOperator(GraphicsState.MiterLimit))
                .Write(new DashOperator(GraphicsState.Dash.Array, GraphicsState.Dash.Phase));

            // write graphic state to stream
        }

        internal void FinishPath()
        {
            // TODO: set colorspace for stroke and/or fill

            if (GraphicsState.Stroke is not null && GraphicsState.Fill is not null)
            {
                if (GraphicsState.Stroke is GrayColor gs)
                    _contentStream.Write(new GrayStrokingColorOperator(gs));
                else if (GraphicsState.Stroke is RgbColor rs)
                    _contentStream.Write(new RgbStrokingColorOperator(rs));
                else if (GraphicsState.Stroke is CmykColor cs)
                    _contentStream.Write(new CmykStrokingColorOperator(cs));
                else
                    throw new NotImplementedException($"The color type {GraphicsState.Stroke.GetType().Name} is not implemented.");

                if (GraphicsState.Fill is GrayColor gf)
                    _contentStream.Write(new GrayNonStrokingColorOperator(gf));
                else if (GraphicsState.Fill is RgbColor rf)
                    _contentStream.Write(new RgbNonStrokingColorOperator(rf));
                else if (GraphicsState.Fill is CmykColor cf)
                    _contentStream.Write(new CmykNonStrokingColorOperator(cf));
                else
                    throw new NotImplementedException($"The color type {GraphicsState.Fill.GetType().Name} is not implemented.");

                _contentStream.Write(new FillAndStrokeOperator(GraphicsState.FillRule));
            }
            else if (GraphicsState.Fill is not null)
            {
                if (GraphicsState.Fill is GrayColor gf)
                    _contentStream.Write(new GrayNonStrokingColorOperator(gf));
                else if (GraphicsState.Fill is RgbColor rf)
                    _contentStream.Write(new RgbNonStrokingColorOperator(rf));
                else if (GraphicsState.Fill is CmykColor cf)
                    _contentStream.Write(new CmykNonStrokingColorOperator(cf));
                else
                    throw new NotImplementedException($"The color type {GraphicsState.Fill.GetType().Name} is not implemented.");

                _contentStream.Write(new FillOperator(GraphicsState.FillRule));
            }
            else if (GraphicsState.Stroke is not null)
            {
                if (GraphicsState.Stroke is GrayColor gs)
                    _contentStream.Write(new GrayStrokingColorOperator(gs));
                else if (GraphicsState.Stroke is RgbColor rs)
                    _contentStream.Write(new RgbStrokingColorOperator(rs));
                else if (GraphicsState.Stroke is CmykColor cs)
                    _contentStream.Write(new CmykStrokingColorOperator(cs));
                else
                    throw new NotImplementedException($"The color type {GraphicsState.Stroke.GetType().Name} is not implemented.");

                _contentStream.Write(new StrokeOperator());
            }
            else
            {
                _contentStream.Write(new EndPathOperator());
            }

            _contentStream.RestoreState();
        }

        public IShapeContext Close()
        {
            _contentStream.Write(new CloseOperator());

            return _context;
        }

        public IPath CurveTo(double cpX1, double cpY1, double cpX2, double cpY2, double finalX, double finalY)
        {
            _contentStream.Write(new CubicBezierCurveDualControlPointsOperator(cpX1, cpY1, cpX2, cpY2, finalX, finalY));
            return this;
        }

        public IPath CurveTo(Point cp1, Point cp2, Point final)
        {
            _contentStream.Write(new CubicBezierCurveDualControlPointsOperator(cp1, cp2, final));
            return this;
        }

        public IPath CurveToWithEndAnker(double cpX, double cpY, double finalX, double finalY)
        {
            _contentStream.Write(new CubicBezierCurveFinalControlPointsOperator(cpX, cpY, finalX, finalY));
            return this;
        }

        public IPath CurveToWithEndAnker(Point cp, Point final)
        {
            _contentStream.Write(new CubicBezierCurveFinalControlPointsOperator(cp, final));
            return this;
        }

        public IPath CurveToWithStartAnker(double cpX, double cpY, double finalX, double finalY)
        {
            _contentStream.Write(new CubicBezierCurveInitialControlPointsOperator(cpX, cpY, finalX, finalY));
            return this;
        }

        public IPath CurveToWithStartAnker(Point cp, Point final)
        {
            _contentStream.Write(new CubicBezierCurveInitialControlPointsOperator(cp, final));
            return this;
        }

        public IPath LineTo(double x, double y)
        {
            _contentStream.Write(new LineOperator(x, y));
            return this;
        }

        public IPath LineTo(Point point)
        {
            _contentStream.Write(new LineOperator(point));
            return this;
        }

        public IPath Move(double x, double y)
        {
            _contentStream.Write(new MoveOperator(x, y));
            return this;
        }

        public IPath Move(Point point)
        {
            _contentStream.Write(new MoveOperator(point));
            return this;
        }

        public IPath Rectangle(double x, double y, double width, double height)
        {
            _contentStream.Write(new RectangleOperator(x, y, width, height));
            return this;
        }

        public IPath Rectangle(Rectangle rectangle)
        {
            _contentStream.Write(new RectangleOperator(rectangle));
            return this;
        }
    }
}
