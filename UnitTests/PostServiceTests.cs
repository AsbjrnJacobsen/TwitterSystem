using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Moq.EntityFrameworkCore;
using PostsService;

namespace TwitterSystemUnitTests;

public class PostServiceTests
{

    [Fact]
    public void GetAccountByIDReturnsAccountOk()
    {
        // === Arrange ===
        // DB context mock
        Mock<PSDBContext> _db = new();
        // Define mock data
        List<Post> postData = new List<Post>()
        {
            new Post() {
                postID = 0,
                username = "Brumbass",
                content =
                    "Brummin' that bass in your ears. Ever since the dawn of man, the wonders of bass have been appreciated." +
                    "From cavemen explorers tipping rocks down valley slopes and being mesmerized by the roars and echos of the rock tumbling down, " +
                    "to lightning cracking in the sky during a storm, causing everyone to bow down in awe at nature's ferocity.",
                repliedToPostID = -1,
            }
        };

        // Setup mock return data
        _db.Setup(db => db.Posts).ReturnsDbSet(postData);

        // Account controller with mock database context
        var ac = new PostController(_db.Object);

        // ===== Act =====
        var dataRet = ac.PostTweet(postData[0]);

        // === Assert ====
        Assert.NotNull(dataRet);
        Assert.Equal(postData[0], (dataRet as OkObjectResult).Value as Post);
    }
}