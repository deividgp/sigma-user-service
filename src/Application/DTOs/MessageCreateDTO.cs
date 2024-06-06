namespace Application.DTOs;

public class MessageCreateDTO
{
    public Guid ConversationId { get; set; }
    public required PartialUser Sender { get; set; }
    public required string Content { get; set; }
}
