using Frete.Application.Services;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;
using Frete.Infrastructure.Persistence;
using Frete.Infrastructure.Repositories;
using Frete.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frete.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFreteInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = "Host=localhost;Port=5432;Database=Frete;Username=postgres;Password=postgres";
        }

        services.AddDbContext<FreteDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<ITabelaFreteRepository, EfTabelaFreteRepository>();
        services.AddScoped<ITabelaFreteClienteRepository, EfTabelaFreteClienteRepository>();
        services.AddScoped<IFaixaFreteRepository, EfFaixaFreteRepository>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<ITabelaFreteApplicationService, TabelaFreteApplicationService>();
        services.AddScoped<ITabelaFreteClienteApplicationService, TabelaFreteClienteApplicationService>();
        services.AddScoped<IFaixaFreteApplicationService, FaixaFreteApplicationService>();

        return services;
    }
}
