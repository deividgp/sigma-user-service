namespace API.Hubs;

public class UserConversationHub(IUserService userService, IConversationService conversationService)
    : Hub
{
    private readonly IUserService _userService = userService;
    private readonly IConversationService _conversationService = conversationService;

    // User
    public async Task<bool> SendContactRequest(SendContactRequestDTO sendContactRequest)
    {
        try
        {
            if (sendContactRequest.TargetUsername == sendContactRequest.Username)
            {
                return false;
            }

            User? aux = await _userService.GetUser(sendContactRequest.TargetUsername);

            if (aux == null)
            {
                return false;
            }

            PartialUserCreateDTO pendingCreate =
                new()
                {
                    UserId = sendContactRequest.UserId,
                    Username = sendContactRequest.Username,
                    TargetUserId = aux.Id,
                    TargetUsername = aux.Username,
                };

            await _userService.AddPending(pendingCreate);
            await Clients
                .User(pendingCreate.TargetUserId.ToString())
                .SendAsync("ReceiveContactRequest", pendingCreate);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendAcceptPending(PartialUserCreateDTO pendingCreate)
    {
        try
        {
            (ContactUser, ContactUser) users = await _userService.AcceptPending(pendingCreate);
            await Clients.Caller.SendAsync("ReceiveAcceptPending", users.Item2);
            await Clients
                .User(pendingCreate.TargetUserId.ToString())
                .SendAsync("ReceiveAcceptPending", users.Item1);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendRemovePending(PartialUserRemoveDTO pendingRemove)
    {
        try
        {
            await _userService.RemovePending(pendingRemove);
            await Clients.Caller.SendAsync("ReceiveRemovePending", pendingRemove);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendRemoveContact(PartialUserRemoveDTO pendingRemove)
    {
        try
        {
            await _userService.RemoveContact(pendingRemove);
            await Clients.Caller.SendAsync("ReceiveRemoveContact", pendingRemove);
            await Clients
                .User(pendingRemove.TargetUserId.ToString())
                .SendAsync(
                    "ReceiveRemoveContact",
                    new PartialUserCreateDTO()
                    {
                        UserId = pendingRemove.TargetUserId,
                        TargetUserId = pendingRemove.UserId,
                    }
                );
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Conversation
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    public async Task SendMessage(MessageCreateDTO messageCreate)
    {
        Message message = await _conversationService.AddMessage(messageCreate);
        await Clients
            .Group(messageCreate.ConversationId.ToString())
            .SendAsync("ReceiveConversationMessage", message);
    }
}
