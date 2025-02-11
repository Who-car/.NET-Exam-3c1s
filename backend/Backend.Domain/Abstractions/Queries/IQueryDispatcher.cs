namespace Backend.Domain.Abstractions.Queries;

public interface IQueryDispatcher
{
    Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) 
        where TQuery : class, IQuery<TResult>;
}