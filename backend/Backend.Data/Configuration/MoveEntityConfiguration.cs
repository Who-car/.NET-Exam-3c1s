using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuration;

public class MoveEntityConfiguration : IEntityTypeConfiguration<Move>
{
    public void Configure(EntityTypeBuilder<Move> builder)
    {
        builder.HasKey(x => new {x.RoomId, x.UserId, x.RoundId});
        
        builder
            .HasOne(x => x.Room)
            .WithMany(x => x.Moves)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(x => x.User)
            .WithMany(x => x.Moves)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(x => x.Round)
            .WithMany(r => r.Moves)
            .HasForeignKey(x => x.RoundId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(x => x.RoomId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RoundId).IsRequired();
        builder.Property(x => x.Value).IsRequired();
        
        builder.Property(x => x.Value)
            .HasConversion<string>(
                x => x.ToString(),
                x => Enum.Parse<MoveType>(x))
            .HasDefaultValue(MoveType.Unknown)
            .IsRequired();
    }
}