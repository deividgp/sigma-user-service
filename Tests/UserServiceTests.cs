using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Services;
using MapsterMapper;
using Moq;

namespace Tests;

public class UserServiceTests
{
    private readonly UserService _userService;
    private readonly Mock<IRepository<User, Guid>> _mockUserRepository;
    private readonly Mock<IConversationService> _mockConversationService;
    private readonly Mock<IMapper> _mockMapper;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IRepository<User, Guid>>();
        _mockConversationService = new Mock<IConversationService>();
        _mockMapper = new Mock<IMapper>();
        _userService = new UserService(
            _mockUserRepository.Object,
            _mockConversationService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetUser_WithValidUserId_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Username = "testuser",
            Email = "testemail"
        };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _userService.GetUser(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
    }

    [Fact]
    public async Task GetUser_WithInvalidUserId_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetUser(userId);

        // Assert
        Assert.Null(result);
    }
}
