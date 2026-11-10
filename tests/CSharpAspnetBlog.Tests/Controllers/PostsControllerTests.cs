using System.Net;
using System.Net.Http.Json;

using AwesomeAssertions;

using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Tests.Utils;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspnetBlog.Tests.Controllers;

public class PostsControllerTests(BlogApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task CreateSavesPostWithCurrentUserAsAuthor()
    {
        var data = new PostCreateDto("my-slug", "My name", "My body");

        var response = await Client.PostAsJsonAsync("/api/posts", data);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var post = await QueryAsync(db => db.Posts.SingleAsync(p => p.Slug == "my-slug"));
        post.Name.Should().Be("My name");
        post.AuthorId.Should().Be(TestUser.Id);
    }

    [Fact]
    public async Task CreateWithoutBodyIsBadRequest()
    {
        var response = await Client.PostAsJsonAsync("/api/posts", new { slug = "s", name = "n" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IndexReturnsAllPosts()
    {
        var post = await CreatePostAsync();

        var response = await Client.GetAsync("/api/posts");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var posts = await response.Content.ReadFromJsonAsync<List<PostDto>>();
        posts.Should().ContainSingle().Which.Id.Should().Be(post.Id);
    }

    [Fact]
    public async Task ShowReturnsPost()
    {
        var post = await CreatePostAsync();

        var dto = await Client.GetFromJsonAsync<PostDto>($"/api/posts/{post.Id}");

        dto.Should().NotBeNull();
        dto.Slug.Should().Be(post.Slug);
        dto.Name.Should().Be(post.Name);
        dto.Body.Should().Be(post.Body);
    }

    [Fact]
    public async Task ShowMissingPostIsNotFound()
    {
        var response = await Client.GetAsync("/api/posts/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateByAuthorChangesPost()
    {
        var post = await CreatePostAsync();

        var response = await Client.PutAsJsonAsync($"/api/posts/{post.Id}", new PostUpdateDto(null, "new name", null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var saved = await QueryAsync(db => db.Posts.SingleAsync(p => p.Id == post.Id));
        saved.Name.Should().Be("new name");
        saved.Body.Should().Be(post.Body);
    }

    [Fact]
    public async Task UpdateByAnotherUserIsForbidden()
    {
        var post = await CreatePostAsync();
        var other = await SaveAsync(ModelGenerator.User());
        using var otherClient = ClientFor(other);

        var response = await otherClient.PutAsJsonAsync($"/api/posts/{post.Id}", new PostUpdateDto(null, "new name", null));

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        var saved = await QueryAsync(db => db.Posts.SingleAsync(p => p.Id == post.Id));
        saved.Name.Should().Be(post.Name);
    }

    [Fact]
    public async Task DestroyByAuthorDeletesPost()
    {
        var post = await CreatePostAsync();

        var response = await Client.DeleteAsync($"/api/posts/{post.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await QueryAsync(db => db.Posts.AnyAsync(p => p.Id == post.Id))).Should().BeFalse();
    }

    [Fact]
    public async Task DestroyByAnotherUserIsForbidden()
    {
        var post = await CreatePostAsync();
        var other = await SaveAsync(ModelGenerator.User());
        using var otherClient = ClientFor(other);

        var response = await otherClient.DeleteAsync($"/api/posts/{post.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        (await QueryAsync(db => db.Posts.AnyAsync(p => p.Id == post.Id))).Should().BeTrue();
    }
}
