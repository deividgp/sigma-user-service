namespace API.Hubs;

public class UserConversationHub(IUserService userService, IConversationService conversationService)
    : Hub
{
    private readonly IUserService _userService = userService;
    private readonly IConversationService _conversationService = conversationService;
    private readonly PushApiClient _pushApiClient = new();

    // User
    public async Task<bool> SendContactRequest(SendContactRequestDTO sendContactRequest)
    {
        try
        {
            if (sendContactRequest.TargetUsername == sendContactRequest.Username)
            {
                return false;
            }

            User? auxUser = await _userService.GetUser(sendContactRequest.TargetUsername);

            if (auxUser == null)
            {
                return false;
            }

            PartialUserCreateDTO pendingCreate =
                new()
                {
                    UserId = sendContactRequest.UserId,
                    Username = sendContactRequest.Username,
                    TargetUserId = auxUser.Id,
                    TargetUsername = auxUser.Username,
                };

            await _userService.AddPending(pendingCreate);
            await Clients
                .User(pendingCreate.TargetUserId.ToString())
                .SendAsync("ReceiveContactRequest", pendingCreate);

            await _pushApiClient.PushSendAsync(
                new PushTicketRequest()
                {
                    PushTo = [auxUser.PushToken],
                    PushTitle = "Contact request from " + sendContactRequest.Username,
                    PushBody = "Accept or reject it",
                    PushPriority = "high"
                }
            );
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

    public async Task<bool> SendJoinServer(ServerCreateDTO serverCreate)
    {
        try
        {
            PartialServer partialServer = await _userService.AddServer(serverCreate);
            await Clients.Caller.SendAsync("ReceiveJoinServer", partialServer);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendLeaveServer(ServerRemoveDTO serverRemove)
    {
        try
        {
            await _userService.RemoveServer(serverRemove);
            await Clients.Caller.SendAsync("ReceiveLeaveServer", serverRemove.ServerId);
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

        Conversation? conversation = await _conversationService.GetConversation(
            messageCreate.ConversationId
        );

        if (conversation is null)
            return;

        Guid userId = conversation
            .UserIds.Where(id => id != messageCreate.Sender.Id)
            .SingleOrDefault();

        User? user = await _userService.GetUser(userId);

        if (user is null)
            return;

        await _pushApiClient.PushSendAsync(
            new PushTicketRequest()
            {
                PushTo = [user.PushToken],
                PushTitle = "New message from " + messageCreate.Sender.Username,
                PushBody = messageCreate.Content,
                PushPriority = "high"
            }
        );
    }
}
