namespace CSharpAspnetBlog.Models;

public class User : BaseEntity
{
    public string Email { get; set; } = "";

    public string FirstName { get; set; } = "";

    public string LastName { get; set; } = "";

    public string PasswordDigest { get; set; } = "";

    public List<Post> Posts { get; set; } = [];
}
