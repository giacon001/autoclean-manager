namespace AutocleanManager.Api.Data;

public static class PriceCalculator
{
    public static decimal CalculateTotalPrice(decimal basePrice, string dirtLevel)
    {
        var normalized = dirtLevel.Trim();

        var multiplier = normalized switch
        {
            "Leve" => 1.00m,
            "Media" => 1.10m,
            "Pesada" => 1.20m,
            _ => 1.00m
        };

        return Math.Round(basePrice * multiplier, 2, MidpointRounding.AwayFromZero);
    }
}
