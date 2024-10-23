using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using TimelineService;

namespace ServiceTests;

public class TimelineServiceTests
{
    [Fact]
    public async void PostTweetReturnsOk()
    {
        // Arrange
        Timeline tlData = new()
        {
           Tweets = new List<Post> {
                new() { username = "AppleTree", content = "Test - Post 1", repliedToPostID = -1, },
                new() { username = "Treefarm", content = "Test - Post 2", repliedToPostID = -1, },
                new() { username = "Bowlboy", content = "Test - Post 3", repliedToPostID = -1, },
                new() { username = "BloodBoy", content = "Test - Post 4", repliedToPostID = -1, },
                new() { username = "Ceo", content = "Test - Post 5", repliedToPostID = -1, },
                new() { username = "BusinessMnger", content = "Test - Post 6", repliedToPostID = -1, },
                new() { username = "Smlr", content = "Test - Post 7", repliedToPostID = -1, },
                new() { username = "Smallr", content = "Test - Post 8", repliedToPostID = -1, },
                new() { username = "Smalr", content = "Test - Post 9", repliedToPostID = -1, },
                new() { username = "Smlr", content = "Test - Post 10", repliedToPostID = -1, }
            }
        };
        
        Mock<TimelineService.Services.TimelineService> timelineServiceMock = new();

        timelineServiceMock.Setup(x => x.Get10()).Returns(async () => tlData);
        
        TimelineController timelineController = new TimelineController(timelineServiceMock.Object);
        
        // Act
        var result = await timelineController.Get10PostForTimeline();
        
        // Assert
        Assert.NotNull(result);
        
        var tlJson = JsonSerializer.Serialize(tlData);
        var resultJson = JsonSerializer.Serialize((result.Result as OkObjectResult).Value);
        
        Assert.IsType<ActionResult<Timeline>>(result);
        Assert.Equal(tlJson, resultJson); 
    }
}