namespace Infrastructure.Services;

public class ConversationService(IRepository<Conversation, Guid> conversationRepository)
    : IConversationService
{
    private readonly IRepository<Conversation, Guid> _conversationRepository =
        conversationRepository;

    public async Task<Conversation?> GetConversation(ConversationDTO conversation)
    {
        return await _conversationRepository.GetSingleAsync(c =>
            c.UserIds.Contains(conversation.UserId1) && c.UserIds.Contains(conversation.UserId2)
        );
    }

    public async Task<Message> AddMessage(MessageCreateDTO messageCreate)
    {
        Message message =
            new()
            {
                SenderId = messageCreate.SenderId,
                Content = messageCreate.Content,
                Timestamp = DateTime.Now
            };
        await _conversationRepository.UpdateOneAsync(
            c => c.Id == messageCreate.ConversationId,
            Builders<Conversation>.Update.Push(c => c.Messages, message)
        );

        return message;
    }

    public async Task<Conversation> CreateConversation(ConversationDTO conversationCreate)
    {
        Conversation conversation =
            new()
            {
                Id = Guid.NewGuid(),
                UserIds = [conversationCreate.UserId1, conversationCreate.UserId2]
            };

        await _conversationRepository.CreateAsync(conversation);

        return conversation;
    }
}
