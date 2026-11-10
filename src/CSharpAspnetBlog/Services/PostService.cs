using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Exceptions;
using CSharpAspnetBlog.Mappers;
using CSharpAspnetBlog.Models;
using CSharpAspnetBlog.Repositories;
using CSharpAspnetBlog.Utils;

namespace CSharpAspnetBlog.Services;

public class PostService(PostRepository posts, UserUtils userUtils)
{
    public async Task<List<PostDto>> GetAllAsync() =>
        (await posts.GetAllAsync()).Select(PostMapper.ToDto).ToList();

    public async Task<PostDto> GetAsync(long id) => PostMapper.ToDto(await FindAsync(id));

    public async Task<PostDto> CreateAsync(PostCreateDto dto)
    {
        await EnsureSlugFreeAsync(dto.Slug);
        var post = PostMapper.ToEntity(dto);
        post.AuthorId = (await userUtils.GetCurrentUserAsync()).Id;
        await posts.AddAsync(post);
        return PostMapper.ToDto(post);
    }

    public async Task<PostDto> UpdateAsync(long id, PostUpdateDto dto)
    {
        var post = await FindAsync(id);
        await EnsureAuthorAsync(post);
        if (dto.Slug is not null && dto.Slug != post.Slug)
        {
            await EnsureSlugFreeAsync(dto.Slug);
        }

        PostMapper.Update(dto, post);
        await posts.UpdateAsync(post);
        return PostMapper.ToDto(post);
    }

    public async Task DeleteAsync(long id)
    {
        var post = await FindAsync(id);
        await EnsureAuthorAsync(post);
        await posts.DeleteAsync(post);
    }

    private async Task<Post> FindAsync(long id) =>
        await posts.FindByIdAsync(id) ?? throw new ResourceNotFoundException($"Post not found: {id}");

    // Менять и удалять пост может только его автор.
    private async Task EnsureAuthorAsync(Post post)
    {
        if ((await userUtils.GetCurrentUserAsync()).Id != post.AuthorId)
        {
            throw new ForbiddenException("Only the author can change the post");
        }
    }

    private async Task EnsureSlugFreeAsync(string slug)
    {
        if (await posts.SlugExistsAsync(slug))
        {
            throw new ConflictException($"Slug already taken: {slug}");
        }
    }
}
