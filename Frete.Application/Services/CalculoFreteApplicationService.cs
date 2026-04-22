using Frete.Application.Common;
using Frete.Application.DTOs;
using Frete.Domain.Interfaces.Repositories;
using Frete.Domain.Interfaces.Services;

namespace Frete.Application.Services;

public sealed class CalculoFreteApplicationService : ICalculoFreteApplicationService
{
    private readonly ITabelaFreteClienteRepository _tabelaFreteClienteRepository;
    private readonly ITabelaFreteRepository _tabelaFreteRepository;
    private readonly IFaixaFreteRepository _faixaFreteRepository;
    private readonly ITenantService _tenantService;

    public CalculoFreteApplicationService(
        ITabelaFreteClienteRepository tabelaFreteClienteRepository,
        ITabelaFreteRepository tabelaFreteRepository,
        IFaixaFreteRepository faixaFreteRepository,
        ITenantService tenantService)
    {
        _tabelaFreteClienteRepository = tabelaFreteClienteRepository;
        _tabelaFreteRepository = tabelaFreteRepository;
        _faixaFreteRepository = faixaFreteRepository;
        _tenantService = tenantService;
    }

    public async Task<CalcularFreteResponse> CalcularAsync(CalcularFreteRequest request, CancellationToken cancellationToken = default)
    {
        if (request.LocalidadeOrigemId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de origem e obrigatoria.", nameof(request.LocalidadeOrigemId));
        }

        if (request.LocalidadeDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Localidade de destino e obrigatoria.", nameof(request.LocalidadeDestinoId));
        }

        if (request.PesoKg <= 0)
        {
            throw new ArgumentException("Peso deve ser maior que zero.", nameof(request.PesoKg));
        }

        var embarcadorId = _tenantService.ObterEmbarcadorIdAtual()
            ?? throw new InvalidOperationException("Embarcador nao identificado.");

        var dataReferencia = request.DataReferencia ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var tabelaFreteCliente = await _tabelaFreteClienteRepository.ObterAplicavelAsync(
            embarcadorId,
            request.LocalidadeOrigemId,
            request.LocalidadeDestinoId,
            dataReferencia,
            cancellationToken);

        if (tabelaFreteCliente is null)
        {
            throw new NotFoundException("Nenhuma tabela de frete ativa encontrada para a rota informada.");
        }

        var tabelaFrete = await _tabelaFreteRepository.ObterPorIdAsync(embarcadorId, tabelaFreteCliente.TabelaFreteId, cancellationToken)
            ?? throw new NotFoundException("Tabela de frete vinculada nao encontrada.");

        var faixas = await _faixaFreteRepository.ListarPorTabelaFreteClienteAsync(embarcadorId, tabelaFreteCliente.Id, cancellationToken);
        var faixaSelecionada = faixas.FirstOrDefault(item => request.PesoKg >= item.LimiteInferiorKg && request.PesoKg <= item.LimiteSuperiorKg);
        if (faixaSelecionada is null)
        {
            throw new NotFoundException("Nenhuma faixa de frete cobre o peso informado.");
        }

        var componentes = request.ComponentesAdicionais ?? Array.Empty<CalculoFreteComponenteRequest>();
        foreach (var componente in componentes)
        {
            if (componente.Valor < 0)
            {
                throw new ArgumentException("Componentes adicionais nao podem ter valor negativo.", nameof(request.ComponentesAdicionais));
            }
        }

        var valorComponentes = componentes.Sum(item => item.Valor);
        var valorBase = faixaSelecionada.Valor;
        var valorCalculado = valorBase + valorComponentes;
        var valorMinimoAplicado = tabelaFreteCliente.ValorMinimo.HasValue && valorCalculado < tabelaFreteCliente.ValorMinimo.Value;
        var valorTotal = valorMinimoAplicado ? tabelaFreteCliente.ValorMinimo!.Value : valorCalculado;

        return new CalcularFreteResponse
        {
            TabelaFreteClienteId = tabelaFreteCliente.Id,
            TabelaFreteId = tabelaFreteCliente.TabelaFreteId,
            TabelaFreteCodigo = tabelaFrete.Codigo,
            TabelaFreteDescricao = tabelaFrete.Descricao,
            DataReferencia = dataReferencia,
            PesoKg = request.PesoKg,
            ValorBase = valorBase,
            ValorComponentes = valorComponentes,
            ValorMinimo = tabelaFreteCliente.ValorMinimo,
            ValorMinimoAplicado = valorMinimoAplicado,
            ValorTotal = valorTotal,
            FaixaSelecionada = new CalculoFreteFaixaResponse
            {
                Id = faixaSelecionada.Id,
                LimiteInferiorKg = faixaSelecionada.LimiteInferiorKg,
                LimiteSuperiorKg = faixaSelecionada.LimiteSuperiorKg,
                Valor = faixaSelecionada.Valor
            }
        };
    }
}
