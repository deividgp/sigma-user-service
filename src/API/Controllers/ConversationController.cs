using System.Threading.Channels;

namespace API.Controllers;

[ApiController]
public class ConversationController(IConversationService conversationService) : ControllerBase
{
    private readonly IConversationService _conversationService = conversationService;

    [HttpGet("/api/Conversation/Get/{conversationId}")]
    public async Task<ActionResult> GetConversation(string conversationId)
    {
        Conversation? conversation = await _conversationService.GetConversation(
            Guid.Parse(conversationId)
        );

        if (conversation == null)
            return NotFound();

        return Ok(conversation);
    }

    [HttpGet("/api/Conversation/GetMessages/{conversationId}/{search}")]
    public async Task<ActionResult> GetMessages(Guid conversationId, string search)
    {
        List<Message>? messages = await _conversationService.GetMessages(
            new MessageGetRequestDTO() { ConversationId = conversationId, Search = search }
        );

        if (messages is null)
            return NotFound();

        return Ok(messages);
    }
}
