using Backend.Domain.Entities;
using MassTransit;

namespace Backend.WebAPI.Consumers;

public class MoveConsumer : IConsumer<Move>
{
    public Task Consume(ConsumeContext<Move> context)
    {
        throw new NotImplementedException();
    }
}