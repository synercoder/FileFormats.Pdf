using Synercoding.FileFormats.Pdf.Content.Colors;

namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Interface to write content to the underlying <see cref="ContentStream"/>
/// </summary>
/// <typeparam name="TSelf">Type should be the implementing type.</typeparam>
public interface IContentContext<TSelf>
    where TSelf : IContentContext<TSelf>
{
    /// <summary>
    ///  Provides access to the raw underlying content stream
    /// </summary>
    ContentStream RawContentStream { get; }

    /// <summary>
    /// Represents the current graphic state
    /// </summary>
    GraphicsState GraphicState { get; }

    /// <summary>
    /// Wrap operations in save and restore state operators, returning a disposable that restores the state when disposed.
    /// </summary>
    /// <returns>An <see cref="IDisposable"/> that restores the state when disposed</returns>
    IDisposable WrapInState();

    /// <summary>
    /// Wrap the <paramref name="contentOperations"/> in save and restore state operators
    /// </summary>
    /// <typeparam name="T">The type of data to pass to <paramref name="contentOperations"/></typeparam>
    /// <param name="data">Data to pass to <paramref name="contentOperations"/></param>
    /// <param name="contentOperations">The operations to execute in the wrapped state</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf WrapInState<T>(T data, Action<T, TSelf> contentOperations);

    /// <summary>
    /// Wrap the <paramref name="contentOperations"/> in save and restore state operators
    /// </summary>
    /// <typeparam name="T">The type of data to pass to <paramref name="contentOperations"/></typeparam>
    /// <param name="data">Data to pass to <paramref name="contentOperations"/></param>
    /// <param name="contentOperations">The operations to execute in the wrapped state</param>
    /// <returns>A task that resolves to this <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    Task<TSelf> WrapInStateAsync<T>(T data, Func<T, TSelf, Task> contentOperations);

    /// <summary>
    /// Concatenate a matrix to <see cref="GraphicsState.CTM"/>
    /// </summary>
    /// <param name="matrix">The matrix to concat</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf ConcatenateMatrix(Matrix matrix);

    /// <summary>
    /// The the <see cref="Color"/> used for stroking operations.
    /// </summary>
    /// <param name="stroke">The color to be used for stroking operations.</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetStroke(Color stroke);

    /// <summary>
    /// The the <see cref="Color"/> used for filling operations.
    /// </summary>
    /// <param name="fill">The color to be used for filling operations.</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetFill(Color fill);

    /// <summary>
    /// Set the line width
    /// </summary>
    /// <param name="lineWidth">The line width to set</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetLineWidth(double lineWidth);

    /// <summary>
    /// Set the line cap
    /// </summary>
    /// <param name="lineCap">The line cap style to set</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetLineCap(LineCapStyle lineCap);

    /// <summary>
    /// Set the line join
    /// </summary>
    /// <param name="lineJoin">The line join style to set</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetLineJoin(LineJoinStyle lineJoin);

    /// <summary>
    /// Set the miter limit
    /// </summary>
    /// <param name="miterLimit">The miter limit to set</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetMiterLimit(double miterLimit);

    /// <summary>
    /// Set the dash pattern
    /// </summary>
    /// <param name="dashPattern">The dash pattern to set</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetDashPattern(Dash dashPattern);

    /// <summary>
    /// Set an extended graphics state (ExtGState) dictionary.
    /// </summary>
    /// <param name="extendedGraphicsState">The state to apply.</param>
    /// <returns>This <see cref="IContentContext{TSelf}"/> to enable chaining operations</returns>
    TSelf SetExtendedGraphicsState(ExtendedGraphicsState extendedGraphicsState);
}


