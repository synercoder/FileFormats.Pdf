using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;

namespace Synercoding.FileFormats.Pdf
{
    public interface IShapeContext
    {
        IShapeContext DefaultState(Action<GraphicsState> configureState);
        IPath NewPath();
        IPath NewPath(Action<GraphicsState> configureState);
    }
}
