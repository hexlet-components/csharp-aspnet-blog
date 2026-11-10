namespace CSharpAspnetBlog.Models;

public class Post : BaseEntity
{
    public long AuthorId { get; set; }

    public User Author { get; set; } = null!;

    public string Slug { get; set; } = "";

    public string Name { get; set; } = "";

    public string Body { get; set; } = "";

    public List<PostComment> Comments { get; set; } = [];
}
