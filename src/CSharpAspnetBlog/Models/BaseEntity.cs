namespace CSharpAspnetBlog.Models;

/// <summary>Общие поля всех сущностей. Даты заполняет <c>BlogDbContext</c>.</summary>
public abstract class BaseEntity
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
