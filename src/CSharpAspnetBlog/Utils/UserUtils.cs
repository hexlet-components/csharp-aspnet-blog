using CSharpAspnetBlog.Exceptions;
using CSharpAspnetBlog.Models;
using CSharpAspnetBlog.Repositories;

using Microsoft.IdentityModel.JsonWebTokens;

namespace CSharpAspnetBlog.Utils;

/// <summary>Достаёт текущего пользователя из JWT запроса.</summary>
public class UserUtils(IHttpContextAccessor accessor, UserRepository users)
{
    public async Task<User> GetCurrentUserAsync()
    {
        var email = accessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? throw new ForbiddenException("Not authenticated");
        return await users.FindByEmailAsync(email)
            ?? throw new ForbiddenException("User does not exist");
    }
}
