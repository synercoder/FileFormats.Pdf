namespace Synercoding.FileFormats.Pdf.Content.Extensions;

/// <summary>
/// Extension class for <see cref="IContentContext{TSelf}"/>
/// </summary>
public static class IContentContextExtensions
{
    /// <summary>
    /// Wraps the operations in <paramref name="contentOperations"/> in save state and restore state operators.
    /// </summary>
    /// <typeparam name="TContext">The context type where this extension applies to.</typeparam>
    /// <param name="context">The content context where that will be wrapped.</param>
    /// <param name="contentOperations">The operations to be wrapped.</param>
    /// <returns>The same <typeparamref name="TContext"/> to enable method chaining.</returns>
    public static TContext WrapInState<TContext>(this TContext context, Action<TContext> contentOperations)
        where TContext : IContentContext<TContext>
        => context.WrapInState(contentOperations, static (operations, context) => operations(context));

    /// <summary>
    /// Wraps the operations in <paramref name="contentOperations"/> in save state and restore state operators.
    /// </summary>
    /// <typeparam name="TContext">The context type where this extension applies to.</typeparam>
    /// <param name="context">The content context where that will be wrapped.</param>
    /// <param name="contentOperations">The async operations to be wrapped.</param>
    /// <returns>Task that resolves to the same <typeparamref name="TContext"/> to enable method chaining.</returns>
    public static Task<TContext> WrapInStateAsync<TContext>(this TContext context, Func<TContext, Task> contentOperations)
        where TContext : IContentContext<TContext>
        => context.WrapInStateAsync(contentOperations, static (operations, context) => operations(context));
}
