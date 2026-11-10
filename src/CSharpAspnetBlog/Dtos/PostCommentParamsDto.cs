namespace CSharpAspnetBlog.Dtos;

/// <summary>Фильтры списка комментариев из query-строки.</summary>
public record PostCommentParamsDto(long? PostId, long? AuthorId, DateTime? CreatedAtGt);
