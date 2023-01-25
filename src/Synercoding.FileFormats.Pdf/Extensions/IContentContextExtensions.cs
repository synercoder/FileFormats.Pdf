using System;
using System.Threading.Tasks;

namespace Synercoding.FileFormats.Pdf.Extensions;
public static class IContentContextExtensions
{
    public static TContext WrapInState<TContext>(this TContext context, Action<TContext> contentOperations)
        where TContext : IContentContext<TContext>
        => context.WrapInState(contentOperations, static (operations, context) => operations(context));

    public static Task<TContext> WrapInStateAsync<TContext>(this TContext context, Func<TContext, Task> contentOperations)
        where TContext: IContentContext<TContext>
        => context.WrapInStateAsync(contentOperations, static (operations, context) => operations(context));
}
