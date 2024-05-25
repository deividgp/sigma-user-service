namespace Domain.Entities;

public class ContactUser : Entity<Guid>
{
    public required string Username { get; set; }
    public required Guid ConversationId { get; set; }
}
