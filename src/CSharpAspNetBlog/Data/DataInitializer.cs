using Bogus;

using CSharpAspNetBlog.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CSharpAspNetBlog.Data;

/// <summary>Создаёт схему и при первом запуске наполняет базу тестовыми данными.</summary>
public class DataInitializer(BlogDbContext db, IPasswordHasher<User> hasher)
{
    public const string DemoEmail = "hexlet@example.com";
    public const string DemoPassword = "qwerty";

    public async Task InitializeAsync()
    {
        await db.Database.EnsureCreatedAsync();

        if (await db.Users.AnyAsync(u => u.Email == DemoEmail))
        {
            return;
        }

        var user = new User { Email = DemoEmail, FirstName = "Hexlet", LastName = "Demo" };
        user.PasswordDigest = hasher.HashPassword(user, DemoPassword);
        db.Users.Add(user);

        var faker = new Faker();
        for (var i = 0; i < 9; i++)
        {
            var post = new Post
            {
                Author = user,
                Name = faker.Lorem.Sentence(3),
                Body = string.Join('\n', faker.Lorem.Paragraphs(2).Split('\n')),
                Slug = $"{faker.Internet.DomainWord()}-{i}",
            };
            post.Comments.Add(new PostComment { Author = user, Body = faker.Lorem.Sentence() });
            db.Posts.Add(post);
        }

        await db.SaveChangesAsync();
    }
}
