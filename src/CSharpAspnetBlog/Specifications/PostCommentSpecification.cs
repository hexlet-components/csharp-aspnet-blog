using System.Linq.Expressions;

using CSharpAspnetBlog.Dtos;
using CSharpAspnetBlog.Models;

namespace CSharpAspnetBlog.Specifications;

/// <summary>Условие выборки комментариев по фильтрам. Пустой фильтр не ограничивает выборку.</summary>
public static class PostCommentSpecification
{
    public static Expression<Func<PostComment, bool>> Build(PostCommentParamsDto filter) =>
        c => (filter.PostId == null || c.PostId == filter.PostId)
            && (filter.AuthorId == null || c.AuthorId == filter.AuthorId)
            && (filter.CreatedAtGt == null || c.CreatedAt > filter.CreatedAtGt);
}
