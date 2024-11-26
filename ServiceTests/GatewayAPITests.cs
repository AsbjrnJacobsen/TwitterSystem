using System.Diagnostics;
using System.Text;
using System.Text.Json;
using GatewayAPI;
using JsonConverter.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.WebEncoders.Testing;
using Models;
using Moq;
using Moq.EntityFrameworkCore;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace ServiceTests;

public class GatewayAPITests : IAsyncLifetime
{
    
    private RetryPollyLayer _retryLayer;

    private WireMockServer _mockServer;
    private Mock<IConfiguration> _mockConfiguration;
    public async Task InitializeAsync()
    {
        _retryLayer = new RetryPollyLayer();
        // Mock Service
        _mockServer = WireMockServer.Start(
            new WireMockServerSettings()
            {
                Urls = new[] { "http://localhost:8080/"}
            });
        
        // Mock IConfiguration
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(x => x["AccountServiceUrl"]).Returns("http://localhost:8080/");
        _mockConfiguration.Setup(x => x["TimelineServiceUrl"]).Returns("http://localhost:8080/");
        _mockConfiguration.Setup(x => x["PostServiceUrl"]).Returns("http://localhost:8080/");
    }

    public async Task DisposeAsync()
    {
        _mockServer.Stop();
    }

    [Fact]
    public async void CreateNewUserReturnsOk()
    {
        // === Arrange ===
        Account fakeAccountData = new()
        {
            Username = "AppleTree",
            Password = "123456",
            Bio = "A very delicious programmer",
            Email = "apple@gmail.com"
        };
        
        // Set up server responses for requests
        _mockServer.Given(Request.Create().WithPath("/api/Account/GetAccountByName/" + fakeAccountData.Username).UsingGet())
            .RespondWith(Response.Create().WithStatusCode(404));
        _mockServer.Given(Request.Create().WithPath("/api/Account/CreateAccount/").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200));
        
        // Create API controller
        APIController apic = new APIController(_mockConfiguration.Object, _retryLayer);

        // ===== Act =====
        var result = await apic.CreateNewUser(fakeAccountData);

        // === Assert ====
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async void Get10PostsReturnsOk()
    {
        //Arrange - Mock DB connection + Fake Posts
        Timeline timelineMock = new Timeline()
        {
            Tweets = new List<Post>
            {
                new() {
                    username = "AppleTree",
                    content = "Test - Post 1", 
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Treefarm",
                    content = "Test - Post 2",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Bowlboy",
                    content = "Test - Post 3",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "SiliconValleyBloodBoy",
                    content = "Test - Post 4",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "SiliconValleyCeo",
                    content = "Test - Post 5",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "BusinessMnger",
                    content = "Test - Post 6",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Smlr",
                    content = "Test - Post 7",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Smallr",
                    content = "Test - Post 8",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Smalr",
                    content = "Test - Post 9",
                    repliedToPostID = -1,
                },
                new()
                {
                    username = "Smlr",
                    content = "Test - Post 10",
                    repliedToPostID = -1,
                }
            }
        };
        
        _mockServer.Given(Request.Create().WithPath("/api/Timeline/Get10PostsForTimeline").UsingGet())
            .RespondWith(Response.Create().WithBodyAsJson(timelineMock));
        
        APIController apic = new APIController(_mockConfiguration.Object, _retryLayer);
        
        //Act
        var result = await apic.Get10Posts();
        var jsonMock = JsonSerializer.Serialize(timelineMock);
        var resultJson = JsonSerializer.Serialize((result.Result as OkObjectResult).Value as Timeline);
        
        //Assert
        Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(jsonMock, resultJson);
    }

    [Fact]
    public async void PostTweetReturnsOk()
    {
        //Arrange
        Post postData = new Post()
        {
            username = "AppleTree",
            content = "Test - Post 1",
            repliedToPostID = -1,
        };
        
        _mockServer.Given(Request.Create().WithPath("/api/Post/PostTweet").UsingPost())
            .RespondWith(Response.Create().WithBodyAsJson(postData));
        
        APIController apic = new APIController(_mockConfiguration.Object, _retryLayer);
        
        //Act
        var result = await apic.PostTweet(postData);
        var postJson = JsonSerializer.Serialize(postData);
        var resultJson = JsonSerializer.Serialize((result as OkObjectResult).Value as Post);
        
        //Assert
        Assert.IsType<OkObjectResult>(result);
        Assert.Equal(postJson, resultJson); 
    }
}