using System.ComponentModel.DataAnnotations;

namespace CSharpAspNetBlog.Dtos;

public record PostCreateDto([Required] string Slug, [Required] string Name, [Required] string Body);
