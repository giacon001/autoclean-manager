namespace AutocleanManager.Api.Models.Requests;

public sealed class CreateWashTypeRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}
