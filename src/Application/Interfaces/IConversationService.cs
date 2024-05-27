namespace Application.Interfaces;

public interface IConversationService
{
    public Task<Conversation?> GetConversation(Guid conversationId);
    public Task<Message> AddMessage(MessageCreateDTO messageCreate);
    public Task<Conversation> CreateConversation(ConversationDTO conversation);
}
