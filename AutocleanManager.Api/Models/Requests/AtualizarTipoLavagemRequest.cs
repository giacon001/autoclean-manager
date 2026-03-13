namespace AutocleanManager.Api.Models.Requests;

public sealed class AtualizarTipoLavagemRequest
{
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public int DuracaoEstimadaMinutos { get; set; }
}
