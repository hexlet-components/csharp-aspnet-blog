using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Exceptions;
using CSharpAspNetBlog.Mappers;
using CSharpAspNetBlog.Models;
using CSharpAspNetBlog.Repositories;
using CSharpAspNetBlog.Specifications;

namespace CSharpAspNetBlog.Services;

public class PostCommentService(PostCommentRepository comments, PostRepository posts, CurrentUserService currentUser)
{
    public const int PageSize = 10;

    public async Task<PageDto<PostCommentDto>> GetPageAsync(PostCommentFilter filter, int page)
    {
        page = Math.Max(page, 1);
        var (items, total) = await comments.GetPageAsync(PostCommentSpecification.Build(filter), page, PageSize);
        return new PageDto<PostCommentDto>(items.Select(PostCommentMapper.ToDto).ToList(), page, PageSize, total);
    }

    public async Task<PageDto<PostCommentDto>> GetPageForPostAsync(long postId, PostCommentFilter filter, int page)
    {
        await EnsurePostExistsAsync(postId);
        return await GetPageAsync(filter with { PostId = postId }, page);
    }

    public async Task<PostCommentDto> GetAsync(long postId, long id) =>
        PostCommentMapper.ToDto(await FindAsync(postId, id));

    public async Task<PostCommentDto> CreateAsync(long postId, PostCommentCreateDto dto)
    {
        await EnsurePostExistsAsync(postId);
        var comment = PostCommentMapper.ToEntity(dto);
        comment.PostId = postId;
        comment.AuthorId = (await currentUser.GetCurrentUserAsync()).Id;
        await comments.AddAsync(comment);
        return PostCommentMapper.ToDto(comment);
    }

    public async Task<PostCommentDto> UpdateAsync(long postId, long id, PostCommentCreateDto dto)
    {
        var comment = await FindAsync(postId, id);
        await EnsureAuthorAsync(comment);
        comment.Body = dto.Body;
        await comments.UpdateAsync(comment);
        return PostCommentMapper.ToDto(comment);
    }

    public async Task DeleteAsync(long postId, long id)
    {
        var comment = await FindAsync(postId, id);
        await EnsureAuthorAsync(comment);
        await comments.DeleteAsync(comment);
    }

    private async Task EnsurePostExistsAsync(long postId)
    {
        if (await posts.FindByIdAsync(postId) is null)
        {
            throw new ResourceNotFoundException($"Post not found: {postId}");
        }
    }

    private async Task<PostComment> FindAsync(long postId, long id)
    {
        var comment = await comments.FindByIdAsync(id);
        return comment is not null && comment.PostId == postId
            ? comment
            : throw new ResourceNotFoundException($"Comment not found: {id}");
    }

    private async Task EnsureAuthorAsync(PostComment comment)
    {
        if ((await currentUser.GetCurrentUserAsync()).Id != comment.AuthorId)
        {
            throw new ForbiddenException("Only the author can change the comment");
        }
    }
}
