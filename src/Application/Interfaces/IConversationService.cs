namespace Application.Interfaces;

public interface IConversationService
{
    public Task<Conversation?> GetConversation(ConversationDTO conversation);
    public Task<Message> AddMessage(MessageCreateDTO messageCreate);
    public Task<Conversation> CreateConversation(ConversationDTO conversation);
}
