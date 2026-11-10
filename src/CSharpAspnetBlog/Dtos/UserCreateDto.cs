using System.ComponentModel.DataAnnotations;

namespace CSharpAspnetBlog.Dtos;

public record UserCreateDto(
    [Required] string FirstName,
    [Required] string LastName,
    [Required, EmailAddress] string Email,
    [Required, StringLength(100, MinimumLength = 3)] string Password);
