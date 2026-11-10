namespace CSharpAspNetBlog.Dtos;

/// <summary>Фильтры списка комментариев из query-строки.</summary>
public record PostCommentFilter(long? PostId, long? AuthorId, DateTime? CreatedAtGt);
