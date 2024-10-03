using System.ComponentModel.DataAnnotations;

namespace Models;

public class Post
{
    [Key]
    [Required]
    public int postID { get; set; }
    [Required]
    public DateTime timestamp { get; private set;} = DateTime.Now;
    public int? repliedToPostID { get; set; } = -1; //if value is -1 then there is no parent tweet
    [Required]
    public string username { get; set; }
    [Required]
    public string content { get; set; }
}