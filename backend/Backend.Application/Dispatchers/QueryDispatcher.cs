using Backend.Domain.Abstractions.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application.Dispatchers;

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : class, IQuery<TResult>
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery,TResult>>();
        return await handler.HandleAsync(query);
    }
}