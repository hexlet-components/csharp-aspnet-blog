using CSharpAspnetBlog.Data;

using Microsoft.Extensions.DependencyInjection;

namespace CSharpAspnetBlog.Tests.Utils;

/// <summary>Запускает инициализатор данных, который в среде Testing не вызывается сам.</summary>
public sealed class DataInitializerRunner(BlogApiFactory factory)
{
    public async Task RunAsync()
    {
        await using var scope = factory.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DataInitializer>().InitializeAsync();
    }
}
