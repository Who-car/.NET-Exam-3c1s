using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configurations;

public class RoundEntityConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder
            .HasOne(x => x.WinnerUser)
            .WithMany(x => x.Rounds)
            .HasForeignKey(x => x.WinnerUserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne(x => x.Room)
            .WithMany(x => x.Rounds)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(x => x.RoomId).IsRequired();
        builder.Property(x => x.WinnerUserId).IsRequired();
    }
}