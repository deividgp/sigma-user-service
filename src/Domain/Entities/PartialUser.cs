namespace Domain.Entities;

public class PartialUser : Entity<Guid>
{
    public required string Username { get; set; }
}
