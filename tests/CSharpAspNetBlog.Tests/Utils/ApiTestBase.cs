using System.Net.Http.Headers;

using CSharpAspNetBlog.Auth;
using CSharpAspNetBlog.Data;
using CSharpAspNetBlog.Models;
using CSharpAspNetBlog.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpAspNetBlog.Tests.Utils;

/// <summary>Общая основа тестов контроллеров: чистая база и клиент с токеном.</summary>
public abstract class ApiTestBase(BlogApiFactory factory) : IClassFixture<BlogApiFactory>, IAsyncLifetime
{
    protected BlogApiFactory Factory { get; } = factory;

    protected User TestUser { get; private set; } = null!;

    protected HttpClient Client { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await db.PostComments.ExecuteDeleteAsync();
        await db.Posts.ExecuteDeleteAsync();
        await db.Users.ExecuteDeleteAsync();

        TestUser = await SaveAsync(ModelGenerator.User());
        Client = ClientFor(TestUser);
    }

    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }

    /// <summary>Клиент, который ходит с JWT указанного пользователя.</summary>
    protected HttpClient ClientFor(User user)
    {
        using var scope = Factory.Services.CreateScope();
        var token = scope.ServiceProvider.GetRequiredService<JwtTokenService>().GenerateToken(user.Email);
        var client = Factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    protected async Task<T> SaveAsync<T>(T entity)
        where T : class
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        db.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    protected async Task<T> QueryAsync<T>(Func<BlogDbContext, Task<T>> query)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        return await query(scope.ServiceProvider.GetRequiredService<BlogDbContext>());
    }

    protected async Task<Post> CreatePostAsync(User? author = null)
    {
        var post = ModelGenerator.Post();
        post.AuthorId = (author ?? TestUser).Id;
        return await SaveAsync(post);
    }
}
