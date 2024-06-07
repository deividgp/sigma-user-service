namespace Infrastructure.Services;

public class UserService(
    IRepository<User, Guid> userRepository,
    IConversationService conversationService,
    IMapper mapper
) : IUserService
{
    private readonly IRepository<User, Guid> _userRepository = userRepository;
    private readonly IConversationService _conversationService = conversationService;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> GetUser(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User?> GetUser(string username)
    {
        return await _userRepository.GetFirstAsync(u => u.Username == username);
    }

    public async Task<(ContactUser, ContactUser)> AddContact(PartialUserCreateDTO contactCreate)
    {
        Conversation conversation = await _conversationService.CreateConversation(
            new ConversationDTO()
            {
                UserId1 = contactCreate.UserId,
                UserId2 = contactCreate.TargetUserId
            }
        );

        ContactUser user =
            new()
            {
                Id = contactCreate.UserId,
                Username = contactCreate.Username!,
                ConversationId = conversation.Id
            };

        ContactUser targetUser =
            new()
            {
                Id = contactCreate.TargetUserId,
                Username = contactCreate.TargetUsername!,
                ConversationId = conversation.Id
            };

        await _userRepository.UpdateOneAsync(
            u => u.Id == user.Id,
            Builders<User>.Update.Push(u => u.Contacts, targetUser)
        );

        await _userRepository.UpdateOneAsync(
            u => u.Id == targetUser.Id,
            Builders<User>.Update.Push(u => u.Contacts, user)
        );

        return (user, targetUser);
    }

    public Task RemoveContact(PartialUserRemoveDTO contactRemove)
    {
        _userRepository.UpdateOneAsync(
            u => u.Id == contactRemove.UserId,
            Builders<User>.Update.PullFilter(
                u => u.Contacts,
                f => f.Id == contactRemove.TargetUserId
            )
        );

        _userRepository.UpdateOneAsync(
            u => u.Id == contactRemove.TargetUserId,
            Builders<User>.Update.PullFilter(u => u.Contacts, f => f.Id == contactRemove.UserId)
        );

        return Task.FromResult(true);
    }

    public async Task<User> CreateUser(UserCreateDTO userCreate)
    {
        User user = _mapper.Map<User>(userCreate);
        await _userRepository.CreateAsync(user);
        return user;
    }

    public async Task AddPending(PartialUserCreateDTO pendingCreate)
    {
        PartialUser user = new() { Id = pendingCreate.UserId, Username = pendingCreate.Username!, };

        await _userRepository.UpdateOneAsync(
            u => u.Id == pendingCreate.TargetUserId,
            Builders<User>.Update.Push(u => u.Pending, user)
        );
    }

    public async Task RemovePending(PartialUserRemoveDTO pendingRemove)
    {
        await _userRepository.UpdateOneAsync(
            u => u.Id == pendingRemove.UserId,
            Builders<User>.Update.PullFilter(
                u => u.Pending,
                f => f.Id == pendingRemove.TargetUserId
            )
        );
    }

    public async Task<(ContactUser, ContactUser)> AcceptPending(PartialUserCreateDTO pendingAccept)
    {
        await RemovePending(
            new PartialUserRemoveDTO()
            {
                UserId = pendingAccept.UserId,
                TargetUserId = pendingAccept.TargetUserId,
            }
        );

        return await AddContact(pendingAccept);
    }

    public Task BlockUser(PartialUserCreateDTO blockCreate)
    {
        throw new NotImplementedException();
    }

    public Task UnblockUser(PartialUserRemoveDTO blockRemove)
    {
        throw new NotImplementedException();
    }

    public async Task<PartialServer> AddServer(ServerCreateDTO serverCreate)
    {
        PartialServer partialServer =
            new()
            {
                Id = serverCreate.ServerId,
                Name = serverCreate.ServerName,
                Icon = serverCreate.Icon
            };

        await _userRepository.UpdateOneAsync(
            u => u.Id == serverCreate.UserId,
            Builders<User>.Update.Push(u => u.Servers, partialServer)
        );

        return partialServer;
    }

    public async Task<Guid> RemoveServer(ServerRemoveDTO serverRemove)
    {
        await _userRepository.UpdateOneAsync(
            u => u.Id == serverRemove.UserId,
            Builders<User>.Update.PullFilter(u => u.Servers, s => s.Id == serverRemove.ServerId)
        );

        return serverRemove.ServerId;
    }

    public async Task<bool> UpdatePushToken(PushTokenUpdateDTO pushTokenUpdate)
    {
        try
        {
            await _userRepository.UpdateOneAsync(u => u.Id == pushTokenUpdate.UserId, Builders<User>.Update.Set(u => u.PushToken, pushTokenUpdate.PushToken));
            return true;
        }
        catch
        {
            return false;
        }
    }
}
