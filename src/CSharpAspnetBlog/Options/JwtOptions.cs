using System.ComponentModel.DataAnnotations;

namespace CSharpAspnetBlog.Options;

/// <summary>Настройки JWT из секции <c>Jwt</c> конфигурации.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required, MinLength(32)]
    public string SecretKey { get; set; } = "";

    [Required]
    public string Issuer { get; set; } = "";

    [Required]
    public string Audience { get; set; } = "";

    [Range(1, 1440)]
    public int ExpiresMinutes { get; set; } = 60;
}
