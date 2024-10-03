using System.ComponentModel.DataAnnotations;

namespace PostsService;

public class Post
{
    [Key]
    [Required]
    public int postID { get; set; }
    
    [Required]
    public DateTime timestamp { get; private set;} = DateTime.Now;
    public int? repliedToPostID { get; set; }
    [Required]
    public string username { get; set; }
    [Required]
    public string content { get; set; }
}