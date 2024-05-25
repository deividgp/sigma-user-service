namespace Domain.Entities;

public class Conversation : Entity<Guid>
{
    public List<Guid> UserIds { get; set; } = [];
    public List<Message> Messages { get; set; } = [];
}
