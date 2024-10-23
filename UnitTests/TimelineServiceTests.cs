using System.Text.Json;
using Castle.Components.DictionaryAdapter.Xml;
using Models;
using Moq;
using Moq.EntityFrameworkCore;

namespace TwitterSystemUnitTests;

public class TimelineServiceTests
{
    [Fact]
    public async void Get10Returns10Posts()
    {
        // === Arrange === 
        Mock<ASDBContext> asDBContext = new Mock<ASDBContext>();
        Mock<PSDBContext> psDBContext = new Mock<PSDBContext>();
    
        var listOfPostsData = new List<Post>
        {
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
        };
        
        // Set up posts data in DbSet
        psDBContext.Setup(x => x.Posts).ReturnsDbSet(listOfPostsData);

        TimelineService.Services.TimelineService tlService = new(asDBContext.Object, psDBContext.Object);

        // ===== Act =====
        var result = await tlService.Get10();
        var resultJson = JsonSerializer.Serialize(result);
        var listOfPostsJson = JsonSerializer.Serialize(new Timeline(){Tweets = listOfPostsData});
        
        // === Assert ====
        Assert.NotNull(result);
        Assert.Equal(listOfPostsJson, resultJson);
    }
}