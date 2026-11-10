using System.Linq.Expressions;

using CSharpAspnetBlog.Data;
using CSharpAspnetBlog.Models;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspnetBlog.Repositories;

public class PostCommentRepository(BlogDbContext db)
{
    public Task<PostComment?> FindByIdAsync(long id) =>
        db.PostComments.FirstOrDefaultAsync(c => c.Id == id);

    /// <summary>Страница комментариев по условию. Номер страницы начинается с 1.</summary>
    public async Task<(List<PostComment> Items, int TotalCount)> GetPageAsync(
        Expression<Func<PostComment, bool>> spec, int page, int pageSize)
    {
        var query = db.PostComments.Where(spec);
        var total = await query.CountAsync();
        var items = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (items, total);
    }

    public async Task AddAsync(PostComment comment)
    {
        db.PostComments.Add(comment);
        await db.SaveChangesAsync();
    }

    public Task UpdateAsync(PostComment comment) => db.SaveChangesAsync();

    public async Task DeleteAsync(PostComment comment)
    {
        db.PostComments.Remove(comment);
        await db.SaveChangesAsync();
    }
}
