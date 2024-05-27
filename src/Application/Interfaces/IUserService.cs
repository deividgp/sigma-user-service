namespace Application.Interfaces;

public interface IUserService
{
    public Task<User> CreateUser(UserCreateDTO userCreate);
    public Task<User?> GetUser(Guid userId);
    public Task<User?> GetUser(string username);
    public Task<(ContactUser, ContactUser)> AddContact(PartialUserCreateDTO contactCreate);
    public Task RemoveContact(PartialUserRemoveDTO contactRemove);
    public Task AddPending(PartialUserCreateDTO pendingCreate);
    public Task RemovePending(PartialUserRemoveDTO pendingRemove);
    public Task<(ContactUser, ContactUser)> AcceptPending(PartialUserCreateDTO pendingAccept);
    public Task BlockUser(PartialUserCreateDTO blockCreate);
    public Task UnblockUser(PartialUserRemoveDTO blockRemove);
    public Task AddServer(Guid userId, Guid serverId);
}
