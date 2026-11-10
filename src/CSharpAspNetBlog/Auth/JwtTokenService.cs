using System.Text;

using CSharpAspNetBlog.Options;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace CSharpAspNetBlog.Auth;

public class JwtTokenService(IOptions<JwtOptions> options)
{
    public static SymmetricSecurityKey CreateKey(JwtOptions jwt) =>
        new(Encoding.UTF8.GetBytes(jwt.SecretKey));

    /// <summary>Выдаёт токен, в <c>sub</c> которого лежит email пользователя.</summary>
    public string GenerateToken(string email)
    {
        var jwt = options.Value;
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            Expires = DateTime.UtcNow.AddMinutes(jwt.ExpiresMinutes),
            Claims = new Dictionary<string, object> { [JwtRegisteredClaimNames.Sub] = email },
            SigningCredentials = new SigningCredentials(CreateKey(jwt), SecurityAlgorithms.HmacSha256),
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
