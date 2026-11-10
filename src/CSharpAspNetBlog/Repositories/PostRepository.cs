using CSharpAspNetBlog.Data;
using CSharpAspNetBlog.Models;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspNetBlog.Repositories;

public class PostRepository(BlogDbContext db)
{
    public Task<List<Post>> GetAllAsync() => db.Posts.OrderBy(p => p.Id).ToListAsync();

    public Task<Post?> FindByIdAsync(long id) => db.Posts.FirstOrDefaultAsync(p => p.Id == id);

    public Task<bool> SlugExistsAsync(string slug) => db.Posts.AnyAsync(p => p.Slug == slug);

    public async Task AddAsync(Post post)
    {
        db.Posts.Add(post);
        await db.SaveChangesAsync();
    }

    public Task UpdateAsync(Post post) => db.SaveChangesAsync();

    public async Task DeleteAsync(Post post)
    {
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
    }
}
