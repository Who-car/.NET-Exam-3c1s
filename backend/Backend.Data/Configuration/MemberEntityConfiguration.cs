using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.Data.Configuration;

public class MemberEntityConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(x => new {x.UserId, x.RoomId});

        builder
            .HasOne(x => x.User)
            .WithMany(u => u.Members)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne(x => x.Room)
            .WithMany(r => r.Members)
            .HasForeignKey(x => x.RoomId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.RoomId).IsRequired();
        
        builder
            .Property(x => x.Role)
            .HasConversion<string>(
                x => x.ToString(), 
                x => Enum.Parse<Role>(x)
            )
            .HasDefaultValue(Role.Spectator);
        
    }
}