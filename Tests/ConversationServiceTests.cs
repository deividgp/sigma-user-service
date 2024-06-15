using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
using Moq;

namespace Tests;

public class ConversationServiceTests
{
    private readonly ConversationService _conversationService;
    private readonly Mock<IRepository<Conversation, Guid>> _mockConversationRepository;

    public ConversationServiceTests()
    {
        _mockConversationRepository = new Mock<IRepository<Conversation, Guid>>();
        _conversationService = new ConversationService(_mockConversationRepository.Object);
    }

    [Fact]
    public async Task GetConversation_WithValidId_ReturnsConversation()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation { Id = conversationId };
        _mockConversationRepository
            .Setup(repo => repo.GetByIdAsync(conversationId))
            .ReturnsAsync(conversation);

        // Act
        var result = await _conversationService.GetConversation(conversationId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(conversationId, result.Id);
    }

    [Fact]
    public async Task GetConversation_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        _mockConversationRepository
            .Setup(repo => repo.GetByIdAsync(conversationId))
            .ReturnsAsync((Conversation)null);

        // Act
        var result = await _conversationService.GetConversation(conversationId);

        // Assert
        Assert.Null(result);
    }
}
