using Frete.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Frete.Infrastructure.Services;

public sealed class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantIdHeader = "X-Tenant-Id";

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? ObterEmbarcadorIdAtual()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return null;
        }

        var claimValue = httpContext.User.FindFirst("EmbarcadorId")?.Value;
        if (Guid.TryParse(claimValue, out var claimTenant))
        {
            return claimTenant;
        }

        if (httpContext.Request.Headers.TryGetValue(TenantIdHeader, out var headerValue)
            && Guid.TryParse(headerValue.ToString(), out var headerTenant))
        {
            return headerTenant;
        }

        return null;
    }
}
