namespace AutocleanManager.Api.Models;

public sealed class Agendamento
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int VeiculoId { get; set; }
    public int TipoLavagemId { get; set; }
    public string NivelSujeira { get; set; } = "Leve";
    public DateTime DataHoraAgendada { get; set; }
    public string Status { get; set; } = "Aguardando";
    public decimal PrecoTotal { get; set; }
}
