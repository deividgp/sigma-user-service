namespace Application.DTOs;

public class MessageCreateDTO
{
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public required string Content { get; set; }
}
