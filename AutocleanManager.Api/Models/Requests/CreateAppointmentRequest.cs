namespace AutocleanManager.Api.Models.Requests;

public sealed class CreateAppointmentRequest
{
    public int UserId { get; set; }
    public int VehicleId { get; set; }
    public int WashTypeId { get; set; }
    public string DirtLevel { get; set; } = "Leve";
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "Aguardando";
}
