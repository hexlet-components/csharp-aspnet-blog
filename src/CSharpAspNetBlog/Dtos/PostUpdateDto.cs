using System.ComponentModel.DataAnnotations;

namespace CSharpAspNetBlog.Dtos;

/// <summary>Частичное обновление: <c>null</c> значит «поле не передано».</summary>
public record PostUpdateDto(
    [MinLength(1)] string? Slug,
    [MinLength(1)] string? Name,
    [MinLength(1)] string? Body);
