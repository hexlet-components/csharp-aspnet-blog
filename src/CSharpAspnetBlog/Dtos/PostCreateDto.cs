using System.ComponentModel.DataAnnotations;

namespace CSharpAspnetBlog.Dtos;

public record PostCreateDto([Required] string Slug, [Required] string Name, [Required] string Body);
