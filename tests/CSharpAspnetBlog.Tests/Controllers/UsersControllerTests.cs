using System.Net;
using System.Net.Http.Json;

using AwesomeAssertions;

using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Tests.Utils;

using Microsoft.EntityFrameworkCore;

namespace CSharpAspnetBlog.Tests.Controllers;

public class UsersControllerTests(BlogApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task CreateSavesUser()
    {
        var data = new UserCreateDto("Mike", "Smith", "mike@example.com", "secret");

        var response = await Client.PostAsJsonAsync("/api/users", data);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var user = await QueryAsync(db => db.Users.SingleAsync(u => u.Email == data.Email));
        user.FirstName.Should().Be("Mike");
        user.PasswordDigest.Should().NotBe("secret");
    }

    [Fact]
    public async Task CreateWithInvalidDataIsBadRequest()
    {
        var data = new UserCreateDto("", "Smith", "not-an-email", "x");

        var response = await Client.PostAsJsonAsync("/api/users", data);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateWithTakenEmailIsConflict()
    {
        var data = new UserCreateDto("Mike", "Smith", TestUser.Email, "secret");

        var response = await Client.PostAsJsonAsync("/api/users", data);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task IndexReturnsAllUsers()
    {
        var response = await Client.GetAsync("/api/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("X-Total-Count").Should().Equal("1");
        var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
        users.Should().ContainSingle().Which.Email.Should().Be(TestUser.Email);
    }

    [Fact]
    public async Task ShowReturnsUser()
    {
        var user = await Client.GetFromJsonAsync<UserDto>($"/api/users/{TestUser.Id}");

        user.Should().BeEquivalentTo(new UserDto(TestUser.Id, TestUser.Email, TestUser.FirstName, TestUser.LastName));
    }

    [Fact]
    public async Task ShowMissingUserIsNotFound()
    {
        var response = await Client.GetAsync("/api/users/999999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateChangesOnlyPassedFields()
    {
        var response = await Client.PutAsJsonAsync($"/api/users/{TestUser.Id}", new UserUpdateDto("Mike", null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await QueryAsync(db => db.Users.SingleAsync(u => u.Id == TestUser.Id));
        user.FirstName.Should().Be("Mike");
        user.LastName.Should().Be(TestUser.LastName);
    }
}
