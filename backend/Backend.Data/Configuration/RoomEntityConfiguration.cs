using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuration;

public class RoomEntityConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder
            .Property(x => x.MaxRating)
            .HasDefaultValue(0);
        
        builder
            .Property(x => x.CreationDate)
            .HasConversion(
                x => x.ToLocalTime(),
                x => x.ToUniversalTime())
            .HasDefaultValue(DateTime.Now.ToUniversalTime())
            .IsRequired();
        
        builder
            .Property(x => x.Status)
            .HasConversion<string>(
                x => x.ToString(),
                x => Enum.Parse<RoomStatus>(x))
            .HasDefaultValue(RoomStatus.Waiting)
            .IsRequired();
        
    }
}