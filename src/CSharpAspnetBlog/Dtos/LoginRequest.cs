using System.ComponentModel.DataAnnotations;

namespace CSharpAspnetBlog.Dtos;

public record LoginRequest([Required] string Username, [Required] string Password);
