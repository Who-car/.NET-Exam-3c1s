using Backend.Domain.Entities;
using MassTransit;

namespace Backend.WebAPI.Consumers;

public class MovesConsumer : IConsumer<Move>
{
    public Task Consume(ConsumeContext<Move> context)
    {
        throw new NotImplementedException();
    }
}