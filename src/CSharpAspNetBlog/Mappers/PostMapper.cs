using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Models;

namespace CSharpAspNetBlog.Mappers;

public static class PostMapper
{
    public static PostDto ToDto(Post post) =>
        new(post.Id, post.AuthorId, post.Slug, post.Name, post.Body, post.CreatedAt);

    public static Post ToEntity(PostCreateDto dto) =>
        new() { Slug = dto.Slug, Name = dto.Name, Body = dto.Body };

    public static void Update(PostUpdateDto dto, Post post)
    {
        post.Slug = dto.Slug ?? post.Slug;
        post.Name = dto.Name ?? post.Name;
        post.Body = dto.Body ?? post.Body;
    }
}
