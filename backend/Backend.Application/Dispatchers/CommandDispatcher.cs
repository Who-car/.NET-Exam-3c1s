﻿using Backend.Domain.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application.Dispatchers;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public async Task DispatchAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command);
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : class, ICommand<TResult>
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command);
    }
}