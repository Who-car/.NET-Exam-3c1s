namespace Backend.Domain.Entities;

public class User : IEntity
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
    
    public List<Member> Members { get; set; }
    public List<Round> Rounds { get; set; }
    public List<Move> Moves { get; set; }
}