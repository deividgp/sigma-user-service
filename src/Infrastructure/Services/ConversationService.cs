namespace Infrastructure.Services;

public class ConversationService(IRepository<Conversation, Guid> conversationRepository)
    : IConversationService
{
    private readonly IRepository<Conversation, Guid> _conversationRepository =
        conversationRepository;

    public async Task<Conversation?> GetConversation(Guid conversationId)
    {
        return await _conversationRepository.GetByIdAsync(conversationId);
    }

    public async Task<Message> AddMessage(MessageCreateDTO messageCreate)
    {
        Message message =
            new()
            {
                Id = Guid.NewGuid(),
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
