using System.ComponentModel.DataAnnotations;

namespace CSharpAspNetBlog.Dtos;

public record PostCommentCreateDto([Required] string Body);
