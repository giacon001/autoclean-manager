namespace AutocleanManager.Api.Models;

public sealed class TipoLavagem
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public int DuracaoEstimadaMinutos { get; set; }
}
