using System.Net;
using System.Net.Http.Json;

using AwesomeAssertions;

using CSharpAspNetBlog.Data;
using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Tests.Utils;

namespace CSharpAspNetBlog.Tests.Controllers;

public class AuthenticationControllerTests(BlogApiFactory factory) : ApiTestBase(factory)
{
    [Fact]
    public async Task LoginReturnsTokenThatOpensProtectedEndpoints()
    {
        await new DataInitializerRunner(Factory).RunAsync();
        using var anonymous = Factory.CreateClient();

        var response = await anonymous.PostAsJsonAsync(
            "/api/login", new LoginRequest(DataInitializer.DemoEmail, DataInitializer.DemoPassword));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadAsStringAsync();
        anonymous.DefaultRequestHeaders.Authorization = new("Bearer", token);
        (await anonymous.GetAsync("/api/posts")).StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task LoginWithWrongPasswordIsUnauthorized()
    {
        await new DataInitializerRunner(Factory).RunAsync();
        using var anonymous = Factory.CreateClient();

        var response = await anonymous.PostAsJsonAsync(
            "/api/login", new LoginRequest(DataInitializer.DemoEmail, "wrong"));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ProtectedEndpointWithoutTokenIsUnauthorized()
    {
        using var anonymous = Factory.CreateClient();

        var response = await anonymous.GetAsync("/api/posts");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
