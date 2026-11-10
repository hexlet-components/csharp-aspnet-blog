using System.ComponentModel.DataAnnotations;

namespace CSharpAspNetBlog.Dtos;

/// <summary>Частичное обновление: <c>null</c> значит «поле не передано».</summary>
public record UserUpdateDto([MinLength(1)] string? FirstName, [MinLength(1)] string? LastName);
