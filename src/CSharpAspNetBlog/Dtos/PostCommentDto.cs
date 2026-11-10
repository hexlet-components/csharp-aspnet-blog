namespace CSharpAspNetBlog.Dtos;

public record PostCommentDto(long Id, long AuthorId, long PostId, string Body, DateTime CreatedAt);
