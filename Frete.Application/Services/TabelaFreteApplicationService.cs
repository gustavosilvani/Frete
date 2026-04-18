using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Domain.Entities;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;

namespace Frete.Application.Services;

public sealed class TabelaFreteApplicationService : ITabelaFreteApplicationService
{
    private readonly ITabelaFreteRepository _repository;
    private readonly ITenantService _tenantService;

    public TabelaFreteApplicationService(ITabelaFreteRepository repository, ITenantService tenantService)
    {
        _repository = repository;
        _tenantService = tenantService;
    }

    public async Task<IReadOnlyCollection<TabelaFreteResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var items = await _repository.ListarPorEmbarcadorAsync(embarcadorId, cancellationToken);
        return items.Select(Mapear).ToList();
    }

    public async Task<TabelaFreteResponse> ObterAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        return Mapear(entity);
    }

    public async Task<TabelaFreteResponse> CriarAsync(CriarTabelaFreteRequest request, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        await ValidarCodigoDuplicadoAsync(embarcadorId, request.Codigo, null, cancellationToken);

        var entity = new TabelaFrete(embarcadorId, request.Codigo, request.Descricao);
        await _repository.AdicionarAsync(entity, cancellationToken);
        return Mapear(entity);
    }

    public async Task<TabelaFreteResponse> AtualizarAsync(Guid id, AtualizarTabelaFreteRequest request, CancellationToken cancellationToken = default)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        await ValidarCodigoDuplicadoAsync(embarcadorId, request.Codigo, entity.Id, cancellationToken);

        entity.Atualizar(request.Codigo, request.Descricao);
        await _repository.AtualizarAsync(entity, cancellationToken);
        return Mapear(entity);
    }

    public async Task<TabelaFreteResponse> AtivarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        entity.Ativar();
        await _repository.AtualizarAsync(entity, cancellationToken);
        return Mapear(entity);
    }

    public async Task<TabelaFreteResponse> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await ObterEntidadeAsync(id, cancellationToken);
        entity.Desativar();
        await _repository.AtualizarAsync(entity, cancellationToken);
        return Mapear(entity);
    }

    private async Task<TabelaFrete> ObterEntidadeAsync(Guid id, CancellationToken cancellationToken)
    {
        var embarcadorId = ObterEmbarcadorIdAtual();
        var entity = await _repository.ObterPorIdAsync(embarcadorId, id, cancellationToken);
        if (entity is null)
        {
            throw new NotFoundException("Tabela de frete não encontrada.");
        }

        return entity;
    }

    private async Task ValidarCodigoDuplicadoAsync(Guid embarcadorId, string codigo, Guid? ignorarId, CancellationToken cancellationToken)
    {
        if (await _repository.CodigoExisteAsync(embarcadorId, codigo, ignorarId, cancellationToken))
        {
            throw new ConflictException("Já existe uma tabela de frete com este código para o embarcador atual.");
        }
    }

    private Guid ObterEmbarcadorIdAtual()
    {
        return _tenantService.ObterEmbarcadorIdAtual()
            ?? throw new InvalidOperationException("Embarcador não identificado.");
    }

    private static TabelaFreteResponse Mapear(TabelaFrete entity)
    {
        return new TabelaFreteResponse
        {
            Id = entity.Id,
            EmbarcadorId = entity.EmbarcadorId,
            Codigo = entity.Codigo,
            Descricao = entity.Descricao,
            Ativo = entity.Ativo,
            CreatedAtUtc = entity.CreatedAtUtc,
            UpdatedAtUtc = entity.UpdatedAtUtc
        };
    }
}
