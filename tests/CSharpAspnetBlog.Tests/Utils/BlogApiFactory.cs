using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace CSharpAspnetBlog.Tests.Utils;

/// <summary>Поднимает приложение с SQLite в памяти. У каждой фабрики своя база.</summary>
public sealed class BlogApiFactory : WebApplicationFactory<Program>
{
    // Пока соединение открыто, база в памяти живёт.
    private readonly SqliteConnection _keepAlive;

    public BlogApiFactory()
    {
        var name = $"Data Source=blog-tests-{Guid.NewGuid():N};Mode=Memory;Cache=Shared";
        _keepAlive = new SqliteConnection(name);
        _keepAlive.Open();
        _connectionString = name;
    }

    private readonly string _connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:Default", _connectionString);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _keepAlive.Dispose();
        }

        base.Dispose(disposing);
    }
}
