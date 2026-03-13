namespace AutocleanManager.Api.Models.Requests;

public sealed class CriarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Papel { get; set; } = "Cliente";
}
