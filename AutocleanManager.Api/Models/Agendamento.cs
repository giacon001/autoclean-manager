namespace AutocleanManager.Api.Models;

public sealed class Agendamento
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public int WashTypeId { get; set; }
    public string DirtLevel { get; set; } = "Leve";
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "Aguardando";
    public decimal TotalPrice { get; set; }
}
