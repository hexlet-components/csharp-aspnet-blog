using CSharpAspNetBlog.Auth;
using CSharpAspNetBlog.Data;
using CSharpAspNetBlog.Handlers;
using CSharpAspNetBlog.Models;
using CSharpAspNetBlog.Options;
using CSharpAspNetBlog.Repositories;
using CSharpAspNetBlog.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<BlogDbContext>((provider, options) =>
    options.UseSqlite(provider.GetRequiredService<IConfiguration>().GetConnectionString("Default")));

builder.Services.AddOptions<JwtOptions>()
    .BindConfiguration(JwtOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>((bearer, jwtOptions) =>
    {
        var jwt = jwtOptions.Value;
        bearer.MapInboundClaims = false;
        bearer.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = JwtTokenService.CreateKey(jwt),
        };
    });

// Всё, кроме /api/login, требует токен, как в Java-версии.
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<DataInitializer>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<PostCommentRepository>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<PostCommentService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DataInitializer>();
    // В среде Testing схему создаёт и данными управляет тест.
    if (app.Environment.IsEnvironment("Testing"))
    {
        await scope.ServiceProvider.GetRequiredService<BlogDbContext>().Database.EnsureCreatedAsync();
    }
    else
    {
        await initializer.InitializeAsync();
    }
}

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi().AllowAnonymous();
app.MapControllers();

await app.RunAsync();

// Нужен тестам: WebApplicationFactory<Program>.
public partial class Program;
