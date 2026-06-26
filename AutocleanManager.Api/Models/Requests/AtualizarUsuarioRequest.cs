namespace AutocleanManager.Api.Models.Requests;

public sealed class AtualizarUsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Papel { get; set; } = string.Empty;

    // Opcional: se vier em branco, mantém a senha atual.
    public string? Senha { get; set; }
}
