namespace CSharpAspNetBlog.Models;

public class PostComment : BaseEntity
{
    public long AuthorId { get; set; }

    public User Author { get; set; } = null!;

    public long PostId { get; set; }

    public Post Post { get; set; } = null!;

    public string Body { get; set; } = "";
}
