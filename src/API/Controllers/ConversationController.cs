namespace API.Controllers;

[ApiController]
public class ConversationController(
    IConversationService conversationService,
    ITokenService tokenService
) : ControllerBase
{
    private readonly IConversationService _conversationService = conversationService;
    private readonly ITokenService _tokenService = tokenService;

    [HttpGet("/api/Conversation/Get")]
    public async Task<ActionResult> GetConversation(string userId)
    {
        ConversationDTO conversation =
            new()
            {
                UserId1 = Guid.Parse(_tokenService.GetClaim("sid")!),
                UserId2 = Guid.Parse(userId)
            };

        return Ok(await _conversationService.GetConversation(conversation));
    }
}
