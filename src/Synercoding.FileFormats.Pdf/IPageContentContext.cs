using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf;

public interface IPageContentContext : IContentContext<IPageContentContext>
{
    IPageContentContext AddImage(Image image);

    IPageContentContext AddText<T>(T data, Action<T, ITextContentContext> textOperations);
    Task<IPageContentContext> AddTextAsync<T>(T data, Func<T, ITextContentContext, Task> textOperations);

    IPageContentContext AddShapes<T>(T data, Action<T, IShapeContext> shapeOperations);
    Task<IPageContentContext> AddShapesAsync<T>(T data, Func<T, IShapeContext, Task> shapeOperations);
}
