using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Exceptions;
using CSharpAspnetBlog.Mappers;
using CSharpAspnetBlog.Models;
using CSharpAspnetBlog.Repositories;
using CSharpAspnetBlog.Specifications;
using CSharpAspnetBlog.Utils;

namespace CSharpAspnetBlog.Services;

public class PostCommentService(PostCommentRepository comments, PostRepository posts, UserUtils userUtils)
{
    public const int PageSize = 10;

    public async Task<PageDto<PostCommentDto>> GetPageAsync(PostCommentParamsDto filter, int page)
    {
        page = Math.Max(page, 1);
        var (items, total) = await comments.GetPageAsync(PostCommentSpecification.Build(filter), page, PageSize);
        return new PageDto<PostCommentDto>(items.Select(PostCommentMapper.ToDto).ToList(), page, PageSize, total);
    }

    public async Task<PageDto<PostCommentDto>> GetPageForPostAsync(long postId, PostCommentParamsDto filter, int page)
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
        comment.AuthorId = (await userUtils.GetCurrentUserAsync()).Id;
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
        if ((await userUtils.GetCurrentUserAsync()).Id != comment.AuthorId)
        {
            throw new ForbiddenException("Only the author can change the comment");
        }
    }
}
