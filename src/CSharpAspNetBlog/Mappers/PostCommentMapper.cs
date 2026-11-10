using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Models;

namespace CSharpAspNetBlog.Mappers;

public static class PostCommentMapper
{
    public static PostCommentDto ToDto(PostComment comment) =>
        new(comment.Id, comment.AuthorId, comment.PostId, comment.Body, comment.CreatedAt);

    public static PostComment ToEntity(PostCommentCreateDto dto) => new() { Body = dto.Body };
}
