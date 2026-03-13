namespace AutocleanManager.Api.Models.Requests;

public sealed class CreateVehicleRequest
{
    public int UserId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Plate { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int? Year { get; set; }
}
