namespace Application.DTOs;

public class MessageGetRequestDTO
{
    public Guid ConversationId { get; set; }
    public required string Search { get; set; }
}
