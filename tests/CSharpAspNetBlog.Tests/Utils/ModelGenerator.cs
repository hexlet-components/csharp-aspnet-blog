using Bogus;

using CSharpAspNetBlog.Models;

namespace CSharpAspNetBlog.Tests.Utils;

/// <summary>Правила генерации тестовых сущностей, аналог ModelGenerator из Java-версии.</summary>
public static class ModelGenerator
{
    public static User User() =>
        new Faker<User>()
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.PasswordDigest, f => f.Internet.Password())
            .Generate();

    public static Post Post() =>
        new Faker<Post>()
            .RuleFor(p => p.Slug, f => f.Internet.DomainWord() + "-" + f.Random.Guid())
            .RuleFor(p => p.Name, f => f.Lorem.Sentence(3))
            .RuleFor(p => p.Body, f => f.Lorem.Paragraph())
            .Generate();

    public static PostComment PostComment() =>
        new Faker<PostComment>()
            .RuleFor(c => c.Body, f => f.Lorem.Sentence())
            .Generate();
}
