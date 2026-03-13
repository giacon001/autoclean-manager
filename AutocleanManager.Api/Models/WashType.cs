namespace AutocleanManager.Api.Models;

public sealed class WashType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}
