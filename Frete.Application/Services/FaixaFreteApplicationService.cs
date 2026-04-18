using Frete.Application.DTOs;
using Frete.Application.Faixas;
using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;

namespace Frete.Application.Services;

public sealed class FaixaFreteApplicationService : IFaixaFreteApplicationService
{
    private readonly IFaixaFreteRepository _faixaFreteRepository;
    private readonly ITenantService _tenantService;

    public FaixaFreteApplicationService(IFaixaFreteRepository faixaFreteRepository, ITenantService tenantService)
    {
        _faixaFreteRepository = faixaFreteRepository;
        _tenantService = tenantService;
    }

    public async Task<IReadOnlyList<FaixaFreteResponse>> ListarAsync(
        Guid tabelaFreteClienteId,
        CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var faixas = await _faixaFreteRepository.ListarPorTabelaFreteClienteAsync(
            embarcadorId,
            tabelaFreteClienteId,
            cancellationToken);

        return faixas.Select(Mapear).ToList();
    }

    public async Task<IReadOnlyList<FaixaFreteResponse>> SubstituirAsync(
        Guid tabelaFreteClienteId,
        IReadOnlyList<FaixaFreteItemRequest> faixas,
        CancellationToken cancellationToken = default)
    {
        FaixaFreteValidator.ValidarSubstituicao(faixas);

        var embarcadorId = ObterEmbarcadorIdAtual();
        var entidades = faixas
            .Select(item => new FaixaFrete(
                tabelaFreteClienteId,
                item.LimiteInferiorKg,
                item.LimiteSuperiorKg,
                item.Valor))
            .ToList();

        await _faixaFreteRepository.SubstituirColecaoAsync(
            embarcadorId,
            tabelaFreteClienteId,
            entidades,
            cancellationToken);

        var gravadas = await _faixaFreteRepository.ListarPorTabelaFreteClienteAsync(
            embarcadorId,
            tabelaFreteClienteId,
            cancellationToken);

        return gravadas.Select(Mapear).ToList();
    }

    private Guid ObterEmbarcadorIdAtual()
    {
        return _tenantService.ObterEmbarcadorIdAtual()
            ?? throw new InvalidOperationException("Embarcador não identificado.");
    }

    private static FaixaFreteResponse Mapear(FaixaFrete entity)
    {
        return new FaixaFreteResponse
        {
            Id = entity.Id,
            TabelaFreteClienteId = entity.TabelaFreteClienteId,
            LimiteInferiorKg = entity.LimiteInferiorKg,
            LimiteSuperiorKg = entity.LimiteSuperiorKg,
            Valor = entity.Valor
        };
    }
}
