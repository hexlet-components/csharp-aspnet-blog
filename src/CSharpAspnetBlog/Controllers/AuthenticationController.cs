using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Services;
using CSharpAspnetBlog.Utils;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAspnetBlog.Controllers;

[ApiController]
[Route("api")]
public class AuthenticationController(UserService users, JwtUtils jwtUtils) : ControllerBase
{
    /// <summary>Возвращает JWT строкой, как Java-версия.</summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Create(LoginRequest request)
    {
        var user = await users.AuthenticateAsync(request.Username, request.Password);
        return user is null ? Unauthorized() : Ok(jwtUtils.GenerateToken(user.Email));
    }
}
