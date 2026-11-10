using CSharpAspNetBlog.Data;
using CSharpAspNetBlog.Models;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspNetBlog.Repositories;

public class UserRepository(BlogDbContext db)
{
    public Task<List<User>> GetAllAsync() => db.Users.OrderBy(u => u.Id).ToListAsync();

    public Task<User?> FindByIdAsync(long id) => db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> FindByEmailAsync(string email) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task AddAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    public Task UpdateAsync(User user) => db.SaveChangesAsync();
}
