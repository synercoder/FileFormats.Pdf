using Synercoding.FileFormats.Pdf.LowLevel;
using Synercoding.FileFormats.Pdf.LowLevel.Colors;
using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf;

public interface IContentContext<TSelf>
    where TSelf : IContentContext<TSelf>
{
    ContentStream RawContentStream { get; }

    TSelf WrapInState<T>(T data, Action<T, TSelf> contentOperations);

    Task<TSelf> WrapInStateAsync<T>(T data, Func<T, TSelf, Task> contentOperations);

    Matrix CTM { get; }
    Color FillColor { get; }
    Color StrokeColor { get; }
    double LineWidth { get; }
    LineCapStyle LineCap { get; }
    LineJoinStyle LineJoin { get; }
    double MiterLimit { get; }
    Dash DashPattern { get; }

    TSelf ConcatenateMatrix(Matrix matrix);

    TSelf SetStroke(Color stroke);
    TSelf SetFill(Color fill);
    TSelf SetLineWidth(double lineWidth);
    TSelf SetLineCap(LineCapStyle lineCap);
    TSelf SetLineJoin(LineJoinStyle lineJoin);
    TSelf SetMiterLimit(double miterLimit);
    TSelf SetDashPattern(Dash dashPattern);
}

