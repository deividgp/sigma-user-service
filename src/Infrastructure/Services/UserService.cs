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

    public async Task AddContact(PartialUserCreateDTO contactCreate)
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

    public Task AddServer(Guid userId, Guid serverId)
    {
        throw new NotImplementedException();
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
                u => u.Contacts,
                f => f.Id == pendingRemove.TargetUserId
            )
        );
    }

    public async Task AcceptPending(PartialUserCreateDTO pendingAccept)
    {
        await RemovePending(
            new PartialUserRemoveDTO()
            {
                UserId = pendingAccept.UserId,
                TargetUserId = pendingAccept.TargetUserId,
            }
        );
        await AddContact(pendingAccept);
    }

    public Task BlockUser(PartialUserCreateDTO blockCreate)
    {
        throw new NotImplementedException();
    }

    public Task UnblockUser(PartialUserRemoveDTO blockRemove)
    {
        throw new NotImplementedException();
    }
}
