namespace Domain.Entities;

public class Message : Entity<Guid>
{
    public required string Content { get; set; }
    public Guid SenderId { get; set; }
    public DateTime Timestamp { get; set; }
}
