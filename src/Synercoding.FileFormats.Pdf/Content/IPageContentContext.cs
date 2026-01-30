namespace Synercoding.FileFormats.Pdf.Content;

/// <summary>
/// Interface to write content to the underlying <see cref="ContentStream"/>
/// </summary>
public interface IPageContentContext : IContentContext<IPageContentContext>
{
    /// <summary>
    /// Add an <see cref="PdfImage"/> to the page
    /// </summary>
    /// <param name="image">The image to paint</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddImage(PdfImage image);

    /// <summary>
    /// Add text to the page
    /// </summary>
    /// <param name="textOperations">The text operations to execute.</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddText(Action<ITextContentContext> textOperations);

    /// <summary>
    /// Add text to the page
    /// </summary>
    /// <typeparam name="T">The type of the data to use in the <paramref name="textOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="textOperations"/>.</param>
    /// <param name="textOperations">The text operations to execute.</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddText<T>(T data, Action<T, ITextContentContext> textOperations);

    /// <summary>
    /// Add text to the page
    /// </summary>
    /// <param name="textOperations">The text operations to execute.</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddTextAsync(Func<ITextContentContext, Task> textOperations);

    /// <summary>
    /// Add text to the page
    /// </summary>
    /// <typeparam name="T">The type of the data to use in the <paramref name="textOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="textOperations"/>.</param>
    /// <param name="textOperations">The text operations to execute.</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddTextAsync<T>(T data, Func<T, ITextContentContext, Task> textOperations);

    /// <summary>
    /// Add shapes to the page
    /// </summary>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddShapes(Action<IShapeContentContext> shapeOperations);

    /// <summary>
    /// Add shapes to the page
    /// </summary>
    /// <typeparam name="T">The type of the data to use in the <paramref name="shapeOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="shapeOperations"/>.</param>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddShapes<T>(T data, Action<T, IShapeContentContext> shapeOperations);

    /// <summary>
    /// Add shapes to the page
    /// </summary>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddShapesAsync(Func<IShapeContentContext, Task> shapeOperations);

    /// <summary>
    /// Add shapes to the page
    /// </summary>
    /// <typeparam name="T">The type of the data to use in the <paramref name="shapeOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="shapeOperations"/>.</param>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddShapesAsync<T>(T data, Func<T, IShapeContentContext, Task> shapeOperations);
}

