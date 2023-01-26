using Synercoding.FileFormats.Pdf.LowLevel;
using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf;

/// <summary>
/// Interface to write content to the underlying <see cref="ContentStream"/>
/// </summary>
public interface IPageContentContext : IContentContext<IPageContentContext>
{
    /// <summary>
    /// Add an <see cref="Image"/> to the page
    /// </summary>
    /// <param name="image">The image to paint</param>
    /// <returns>This <see cref="IPageContentContext"/> to enable chaining operations</returns>
    IPageContentContext AddImage(Image image);

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
    /// <typeparam name="T">The type of the data to use in the <paramref name="textOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="textOperations"/>.</param>
    /// <param name="textOperations">The text operations to execute.</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddTextAsync<T>(T data, Func<T, ITextContentContext, Task> textOperations);

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
    /// <typeparam name="T">The type of the data to use in the <paramref name="shapeOperations"/></typeparam>
    /// <param name="data">Data to use in the <paramref name="shapeOperations"/>.</param>
    /// <param name="shapeOperations">The shape operations to execute</param>
    /// <returns>A task that resolves to this <see cref="IPageContentContext"/> to enable chaining operations</returns>
    Task<IPageContentContext> AddShapesAsync<T>(T data, Func<T, IShapeContentContext, Task> shapeOperations);
}
