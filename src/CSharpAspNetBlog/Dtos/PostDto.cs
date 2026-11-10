namespace CSharpAspNetBlog.Dtos;

public record PostDto(long Id, long AuthorId, string Slug, string Name, string Body, DateTime CreatedAt);
