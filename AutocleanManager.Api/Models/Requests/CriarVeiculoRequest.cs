namespace AutocleanManager.Api.Models.Requests;

public sealed class CriarVeiculoRequest
{
    public int UsuarioId { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Placa { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public int? Ano { get; set; }
}
