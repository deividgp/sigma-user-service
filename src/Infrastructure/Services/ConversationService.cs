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
                Sender = messageCreate.Sender,
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

    public async Task<List<Message>?> GetMessages(MessageGetRequestDTO messageGetRequest)
    {
        Conversation? conversation = await _conversationRepository.GetByIdAsync(
            messageGetRequest.ConversationId
        );

        if (conversation is null)
            return null;

        return conversation
            .Messages.Where(m =>
                m.Content.Contains(
                    messageGetRequest.Search,
                    StringComparison.CurrentCultureIgnoreCase
                )
            )
            .ToList();
    }
}
