using System.ComponentModel.DataAnnotations;

namespace PostsService;

public class Post
{
    [Key]
    [Required]
    public int postID { get; set; }
    [Required]
    public DateTime timestamp { get; set; }
    public int? repliedToPostID { get; set; }
    [Required]
    public string username { get; set; }
    [Required]
    public string content { get; set; }
}