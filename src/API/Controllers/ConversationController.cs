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
}
