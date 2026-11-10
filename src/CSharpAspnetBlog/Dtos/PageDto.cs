namespace CSharpAspnetBlog.Dtos;

/// <summary>Страница результатов. Номер страницы начинается с 1.</summary>
public record PageDto<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
}
