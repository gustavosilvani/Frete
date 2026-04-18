using System.Security.Claims;
using System.Text.Encodings.Web;
using Frete.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frete.Tests;

public sealed class FreteApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"frete-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<FreteDbContext>();
            services.RemoveAll<DbContextOptions<FreteDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<FreteDbContext>>();

            services.AddDbContext<FreteDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}

internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string TenantHeader = "X-Test-Embarcador-Id";
    public const string RolesHeader = "X-Test-Roles";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var embarcadorId = Request.Headers[TenantHeader].ToString();
        if (string.IsNullOrWhiteSpace(embarcadorId))
        {
            embarcadorId = Guid.NewGuid().ToString();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "test-user"),
            new("EmbarcadorId", embarcadorId)
        };

        var roles = Request.Headers[RolesHeader].ToString();
        foreach (var role in roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
