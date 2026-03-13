namespace AutocleanManager.Api.Models.Requests;

public sealed class AtualizarAgendamentoRequest
{
    public int UsuarioId { get; set; }
    public int VeiculoId { get; set; }
    public int TipoLavagemId { get; set; }
    public string NivelSujeira { get; set; } = string.Empty;
    public DateTime DataHoraAgendada { get; set; }
    public string Status { get; set; } = string.Empty;
}
