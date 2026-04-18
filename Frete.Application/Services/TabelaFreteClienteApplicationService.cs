using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;

namespace Frete.Application.Services;

public sealed class TabelaFreteClienteApplicationService : ITabelaFreteClienteApplicationService
{
    private readonly ITabelaFreteClienteRepository _tabelaFreteClienteRepository;
    private readonly ITabelaFreteRepository _tabelaFreteRepository;
    private readonly ITenantService _tenantService;

    public TabelaFreteClienteApplicationService(
        ITabelaFreteClienteRepository tabelaFreteClienteRepository,
        ITabelaFreteRepository tabelaFreteRepository,
        ITenantService tenantService)
    {
        _tabelaFreteClienteRepository = tabelaFreteClienteRepository;
        _tabelaFreteRepository = tabelaFreteRepository;
        _tenantService = tenantService;
    }

    public async Task<IReadOnlyCollection<TabelaFreteClienteResponse>> ListarAsync(Guid? tabelaFreteId = null, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var clientes = await _tabelaFreteClienteRepository.ListarPorEmbarcadorAsync(embarcadorId, tabelaFreteId, cancellationToken);
        var tabelas = await _tabelaFreteRepository.ListarPorEmbarcadorAsync(embarcadorId, cancellationToken);
        var mapaTabelas = tabelas.ToDictionary(item => item.Id);

        return clientes.Select(item => Mapear(item, mapaTabelas)).ToList();
    }

    public async Task<TabelaFreteClienteResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        var tabela = await _tabelaFreteRepository.ObterPorIdAsync(embarcadorId, entity.TabelaFreteId, cancellationToken)
            ?? throw new NotFoundException("Tabela de frete vinculada não encontrada.");

        return Mapear(entity, new Dictionary<Guid, TabelaFrete> { [tabela.Id] = tabela });
    }

    public async Task<TabelaFreteClienteResponse> CriarAsync(CriarTabelaFreteClienteRequest request, CancellationToken cancellationToken = default)
    {
        var tabela = await ObterTabelaFreteAsync(request.TabelaFreteId, cancellationToken);
        var entity = new TabelaFreteCliente(
            tabela.EmbarcadorId,
            request.TabelaFreteId,
            request.LocalidadeOrigemId,
            request.LocalidadeDestinoId,
            request.VigenciaInicio,
            request.VigenciaFim);

        await _tabelaFreteClienteRepository.AdicionarAsync(entity, cancellationToken);
        return Mapear(entity, new Dictionary<Guid, TabelaFrete> { [tabela.Id] = tabela });
    }

    public async Task<TabelaFreteClienteResponse> AtualizarAsync(Guid id, AtualizarTabelaFreteClienteRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        var tabela = await ObterTabelaFreteAsync(request.TabelaFreteId, cancellationToken);

        entity.Atualizar(
            request.TabelaFreteId,
            request.LocalidadeOrigemId,
            request.LocalidadeDestinoId,
            request.VigenciaInicio,
            request.VigenciaFim);

        await _tabelaFreteClienteRepository.AtualizarAsync(entity, cancellationToken);
        return Mapear(entity, new Dictionary<Guid, TabelaFrete> { [tabela.Id] = tabela });
    }

    public async Task<TabelaFreteClienteResponse> AtivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        entity.Ativar();
        await _tabelaFreteClienteRepository.AtualizarAsync(entity, cancellationToken);

        var tabela = await _tabelaFreteRepository.ObterPorIdAsync(embarcadorId, entity.TabelaFreteId, cancellationToken)
            ?? throw new NotFoundException("Tabela de frete vinculada não encontrada.");

        return Mapear(entity, new Dictionary<Guid, TabelaFrete> { [tabela.Id] = tabela });
    }

    public async Task<TabelaFreteClienteResponse> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        entity.Desativar();
        await _tabelaFreteClienteRepository.AtualizarAsync(entity, cancellationToken);

        var tabela = await _tabelaFreteRepository.ObterPorIdAsync(embarcadorId, entity.TabelaFreteId, cancellationToken)
            ?? throw new NotFoundException("Tabela de frete vinculada não encontrada.");

        return Mapear(entity, new Dictionary<Guid, TabelaFrete> { [tabela.Id] = tabela });
    }

    private async Task<TabelaFreteCliente> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await _tabelaFreteClienteRepository.ObterPorIdAsync(embarcadorId, id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("Tabela de frete cliente não encontrada.");
        }

        return entity;
    }

    private async Task<TabelaFrete> ObterTabelaFreteAsync(Guid tabelaFreteId, CancellationToken cancellationToken)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var tabela = await _tabelaFreteRepository.ObterPorIdAsync(embarcadorId, tabelaFreteId, cancellationToken);
        if (tabela is null)
        {
            throw new NotFoundException("Tabela de frete não encontrada.");
        }

        return tabela;
    }

    private Guid ObterEmbarcadorIdAtual()
    {
        return _tenantService.ObterEmbarcadorIdAtual()
            ?? throw new InvalidOperationException("Embarcador não identificado.");
    }

    private static TabelaFreteClienteResponse Mapear(TabelaFreteCliente entity, IReadOnlyDictionary<Guid, TabelaFrete> tabelas)
    {
        tabelas.TryGetValue(entity.TabelaFreteId, out var tabela);

        return new TabelaFreteClienteResponse
        {
            Id = entity.Id,
            EmbarcadorId = entity.EmbarcadorId,
            TabelaFreteId = entity.TabelaFreteId,
            TabelaFreteCodigo = tabela?.Codigo ?? string.Empty,
            TabelaFreteDescricao = tabela?.Descricao ?? string.Empty,
            LocalidadeOrigemId = entity.LocalidadeOrigemId,
            LocalidadeDestinoId = entity.LocalidadeDestinoId,
            VigenciaInicio = entity.VigenciaInicio,
            VigenciaFim = entity.VigenciaFim,
            Ativo = entity.Ativo,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
