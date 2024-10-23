using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Models;

public class Post
{
    [Key]
    [Required]
    [JsonPropertyName("post_id")]
    public int postID { get; set; }
    [Required]
    [JsonPropertyName("timestamp")]
    public DateTime timestamp { get; init; } = DateTime.Now;
    [JsonPropertyName("replied_to_post")]
    public int? repliedToPostID { get; set; } = -1; //if value is -1 then there is no parent tweet
    [Required]
    [JsonPropertyName("username")]
    public string username { get; set; }
    [Required]
    [JsonPropertyName("content")]
    public string content { get; set; }
}