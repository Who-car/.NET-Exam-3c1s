using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class PostgresDbContext(DbContextOptions<PostgresDbContext> options) : DbContext(options)
{
    public DbSet<Member> Members { get; set; }
    public DbSet<Move> Moves { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Round> Rounds { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}