using System.Data;
using Dapper;
using Galaxy.Conqueror.API.Models;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.API.Configuration.Database;
using Moq;
using Moq.Dapper;

namespace Galaxy.Conqueror.Test.Services;
namespace Galaxy.Conqueror.API.Models;


public class UserServiceTests
{
    private readonly Mock<IDbConnectionFactory> _dbMock;
    private readonly Mock<ISetupService> _setupServiceMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _dbMock = new Mock<IDbConnectionFactory>();
        _setupServiceMock = new Mock<ISetupService>();
        _userService = new UserService(_dbMock.Object, _setupServiceMock.Object);
    }

    [Fact]
    public async Task GetUserById_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User { Id = userId, Email = "user@example.com" };

        _dbMock.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.GetUserById(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result!.Id);
    }

    [Fact]
    public async Task CreateUser_ReturnsExistingUser_WhenEmailExists()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Email = "existing@example.com" };
        var userInfo = new UserInfoResponse { email = existingUser.Email, sub = "1234" };

        _dbMock.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<User>(
            It.Is<string>(s => s.Contains("SELECT * FROM users WHERE email")),
            It.IsAny<object>(),
            null, null, null))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.CreateUser(userInfo);

        // Assert
        Assert.Equal(existingUser.Id, result!.Id);
    }

    [Fact]
    public async Task CreateUser_CreatesNewUser_WhenNotExists()
    {
        // Arrange
        var newUser = new User { Id = Guid.NewGuid(), Email = "new@example.com" };
        var userInfo = new UserInfoResponse { email = newUser.Email, sub = "google-sub-id" };

        _dbMock.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null))
            .ReturnsAsync((User)null!); // simulate not found

        _setupServiceMock.Setup(s => s.SetupPlayerDefaults(
            userInfo.email, userInfo.sub, "Galaxy Conqueror"))
            .ReturnsAsync(newUser);

        // Act
        var result = await _userService.CreateUser(userInfo);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newUser.Email, result!.Email);
    }

    [Fact]
    public async Task UpdateUser_UpdatesAndReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updatedUser = new User { Id = userId, Email = "updated@example.com", Username = "newname" };

        _dbMock.SetupDapperAsync(c => c.QuerySingleOrDefaultAsync<User>(
            It.Is<string>(sql => sql.Contains("UPDATE users")),
            It.IsAny<object>(),
            null, null, null))
            .ReturnsAsync(updatedUser);

        // Act
        var result = await _userService.UpdateUser(userId, "newname");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("newname", result!.Username);
    }

    [Fact]
    public async Task DeleteUser_ExecutesWithoutError()
    {
        // Arrange
        _dbMock.SetupDapperAsync(c => c.ExecuteAsync(
            It.Is<string>(sql => sql.Contains("DELETE")),
            It.IsAny<object>(),
            null, null, null))
            .ReturnsAsync(1);

        // Act
        var exception = await Record.ExceptionAsync(() => _userService.DeleteUser("id"));

        // Assert
        Assert.Null(exception);
    }
}
