using System.Net;
using System.Net.Http.Json;

using AwesomeAssertions;

using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Models;
using CSharpAspnetBlog.Services;
using CSharpAspnetBlog.Tests.Utils;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspnetBlog.Tests.Controllers;

public class PostsCommentsControllerTests(BlogApiFactory factory) : ApiTestBase(factory)
{
    private async Task<PostComment> CreateCommentAsync(Post post, User? author = null, DateTime? createdAt = null)
    {
        var comment = ModelGenerator.PostComment();
        comment.PostId = post.Id;
        comment.AuthorId = (author ?? TestUser).Id;
        if (createdAt is not null)
        {
            comment.CreatedAt = createdAt.Value;
        }

        return await SaveAsync(comment);
    }

    [Fact]
    public async Task IndexFiltersByPostId()
    {
        var post = await CreatePostAsync();
        var post2 = await CreatePostAsync();
        await CreateCommentAsync(post);
        await CreateCommentAsync(post2);

        var page = await Client.GetFromJsonAsync<PageDto<PostCommentDto>>($"/api/posts_comments?postId={post.Id}");

        page.Should().NotBeNull();
        page.Items.Should().ContainSingle().Which.PostId.Should().Be(post.Id);
    }

    [Fact]
    public async Task IndexFiltersByCreatedAtGt()
    {
        var post = await CreatePostAsync();
        await CreateCommentAsync(post, createdAt: new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        var fresh = await CreateCommentAsync(post, createdAt: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var page = await Client.GetFromJsonAsync<PageDto<PostCommentDto>>(
            "/api/posts_comments?createdAtGt=2023-01-01T00:00:00Z");

        page.Should().NotBeNull();
        page.Items.Should().ContainSingle().Which.Id.Should().Be(fresh.Id);
    }

    [Fact]
    public async Task IndexPaginatesByTen()
    {
        var post = await CreatePostAsync();
        for (var i = 0; i < PostCommentService.PageSize + 2; i++)
        {
            await CreateCommentAsync(post);
        }

        var first = await Client.GetFromJsonAsync<PageDto<PostCommentDto>>($"/api/posts/{post.Id}/comments");
        var second = await Client.GetFromJsonAsync<PageDto<PostCommentDto>>($"/api/posts/{post.Id}/comments?page=2");

        first.Should().NotBeNull();
        first.Items.Should().HaveCount(10);
        first.TotalCount.Should().Be(12);
        second.Should().NotBeNull();
        second.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task IndexOfMissingPostIsNotFound()
    {
        var response = await Client.GetAsync("/api/posts/999999/comments");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateAddsCommentToPost()
    {
        var post = await CreatePostAsync();

        var response = await Client.PostAsJsonAsync($"/api/posts/{post.Id}/comments", new PostCommentCreateDto("Nice"));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var comment = await QueryAsync(db => db.PostComments.SingleAsync());
        comment.Body.Should().Be("Nice");
        comment.PostId.Should().Be(post.Id);
        comment.AuthorId.Should().Be(TestUser.Id);
    }

    [Fact]
    public async Task CreateWithEmptyBodyIsBadRequest()
    {
        var post = await CreatePostAsync();

        var response = await Client.PostAsJsonAsync($"/api/posts/{post.Id}/comments", new PostCommentCreateDto(""));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAndDestroyByAnotherUserAreForbidden()
    {
        var post = await CreatePostAsync();
        var comment = await CreateCommentAsync(post);
        var other = await SaveAsync(ModelGenerator.User());
        using var otherClient = ClientFor(other);
        var url = $"/api/posts/{post.Id}/comments/{comment.Id}";

        (await otherClient.PutAsJsonAsync(url, new PostCommentCreateDto("hack"))).StatusCode
            .Should().Be(HttpStatusCode.Forbidden);
        (await otherClient.DeleteAsync(url)).StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task AuthorUpdatesAndDeletesComment()
    {
        var post = await CreatePostAsync();
        var comment = await CreateCommentAsync(post);
        var url = $"/api/posts/{post.Id}/comments/{comment.Id}";

        (await Client.PutAsJsonAsync(url, new PostCommentCreateDto("edited"))).StatusCode
            .Should().Be(HttpStatusCode.OK);
        (await Client.GetFromJsonAsync<PostCommentDto>(url))!.Body.Should().Be("edited");
        (await Client.DeleteAsync(url)).StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await Client.GetAsync(url)).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
