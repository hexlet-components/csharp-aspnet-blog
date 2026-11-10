using CSharpAspNetBlog.Auth;
using CSharpAspNetBlog.Dtos;
using CSharpAspNetBlog.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAspNetBlog.Controllers;

[ApiController]
[Route("api")]
public class AuthenticationController(UserService users, JwtTokenService jwtTokens) : ControllerBase
{
    /// <summary>Возвращает JWT строкой, как Java-версия.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Create(LoginRequest request)
    {
        var user = await users.AuthenticateAsync(request.Username, request.Password);
        return user is null ? Unauthorized() : Ok(jwtTokens.GenerateToken(user.Email));
    }
}
