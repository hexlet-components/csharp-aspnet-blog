using System.ComponentModel.DataAnnotations;

namespace CSharpAspnetBlog.Dtos;

public record PostCommentCreateDto([Required] string Body);
