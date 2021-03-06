using Synercoding.FileFormats.Pdf.LowLevel.Graphics;
using System;

namespace Synercoding.FileFormats.Pdf
{
    /// <summary>
    /// Interface representing a context which can be used to draw shapes on a page
    /// </summary>
    public interface IShapeContext
    {
        /// <summary>
        /// Change the default graphics state
        /// </summary>
        /// <param name="configureState">Action used to configure the <see cref="GraphicsState"/></param>
        /// <returns>The calling <see cref="IShapeContext"/> to support chaining</returns>
        IShapeContext DefaultState(Action<GraphicsState> configureState);

        /// <summary>
        /// Start a new <see cref="IPath"/>
        /// </summary>
        /// <returns>The new <see cref="IPath"/> object to chain pathing operators</returns>
        IPath NewPath();

        /// <summary>
        /// Start a new <see cref="IPath"/> with a different graphics state
        /// </summary>
        /// <param name="configureState">The action used to change the <see cref="GraphicsState"/></param>
        /// <returns>The new <see cref="IPath"/> object to chain pathing operators</returns>
        IPath NewPath(Action<GraphicsState> configureState);
    }
}
