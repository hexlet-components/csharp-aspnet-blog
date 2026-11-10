using System.ComponentModel.DataAnnotations;

namespace CSharpAspNetBlog.Dtos;

public record LoginRequest([Required] string Username, [Required] string Password);
