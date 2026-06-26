using System.Text.Json.Serialization;

namespace AutocleanManager.Api.Models;

public sealed class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Papel { get; set; } = "Cliente";

    // A senha nunca volta nas respostas da API (apenas entra nos cadastros).
    [JsonIgnore]
    public string Senha { get; set; } = string.Empty;
}
