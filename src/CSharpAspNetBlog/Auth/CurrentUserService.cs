using CSharpAspNetBlog.Exceptions;
using CSharpAspNetBlog.Models;
using CSharpAspNetBlog.Repositories;

using Microsoft.IdentityModel.JsonWebTokens;

namespace CSharpAspNetBlog.Auth;

/// <summary>Достаёт текущего пользователя из JWT запроса.</summary>
public class CurrentUserService(IHttpContextAccessor accessor, UserRepository users)
{
    public async Task<User> GetCurrentUserAsync()
    {
        var email = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new ForbiddenException("Not authenticated");
        return await users.FindByEmailAsync(email)
            ?? throw new ForbiddenException("User does not exist");
    }
}
