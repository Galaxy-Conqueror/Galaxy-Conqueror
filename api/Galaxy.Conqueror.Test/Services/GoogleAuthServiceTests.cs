using System;
using System.Collections.Generic;
using System.Net.Http;
using Galaxy.Conqueror.API.Models.Database;
using Galaxy.Conqueror.API.Models.Responses;
using Galaxy.Conqueror.API.Services;
using Galaxy.Conqueror.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace Galaxy.Conqueror.Tests.Unit.Services;

public class GoogleAuthServiceTests : IClassFixture<PostgreSqlFixture>, IAsyncLifetime
{
    private readonly PostgreSqlFixture fixture;

    private readonly Mock<HttpMessageHandler> httpMessageHandlerMock;
    private readonly HttpClient httpClient;
    private readonly Mock<IHostEnvironment> envMock;
    private readonly IConfiguration config;
    private readonly GoogleAuthService authService;

    public GoogleAuthServiceTests(PostgreSqlFixture fixture)
    {
        this.fixture = fixture;

        httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        httpClient = new HttpClient(httpMessageHandlerMock.Object);

        envMock = new Mock<IHostEnvironment>();
        envMock.Setup(e => e.EnvironmentName).Returns("Development");

        config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Google:ClientId"] = "test-client-id",
                ["Google:ClientSecret"] = "test-client-secret",
                ["Google:RedirectUri"] = "http://localhost/callback"
            })
            .Build();

        var mockUserService = new Mock<IUserService>();
        mockUserService
            .Setup(s => s.CreateUser(It.IsAny<UserInfoResponse>()))
            .ReturnsAsync((UserInfoResponse userInfo) => new User
            {
                Id = Guid.NewGuid(),
                Email = userInfo.email,
                GoogleId = userInfo.sub
            });

        authService = new GoogleAuthService(httpClient, mockUserService.Object, config, envMock.Object);

    }

    public async Task InitializeAsync() => await fixture.TruncateTables();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Login_ShouldExchangeTokenAndReturnUser()
    {
        var authCode = "fake-auth-code";

        var fakeTokenResponse = new TokenResponse
        {
            access_token = "test-access-token",
            id_token = "test-id-token"
        };

        var fakeUserInfo = new UserInfoResponse
        {
            email = "test@example.com",
            sub = Guid.NewGuid().ToString()
        };

        httpMessageHandlerMock.SetupJsonResponse(HttpMethod.Post, "https://oauth2.googleapis.com/token", fakeTokenResponse);
        httpMessageHandlerMock.SetupJsonResponse(HttpMethod.Get, $"https://www.googleapis.com/oauth2/v3/tokeninfo?access_token={fakeTokenResponse.access_token}", fakeUserInfo);

        var result = await authService.Login(authCode);

        Assert.NotNull(result);
        Assert.NotNull(result.User);

        Assert.Equal(fakeUserInfo.email, result.User.Email);
        Assert.Equal(fakeUserInfo.sub, result.User.GoogleId);
        Assert.Equal(fakeTokenResponse.id_token, result.JWT);
    }
}
