namespace AutocleanManager.Api.Models;

public sealed class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Papel { get; set; } = "Cliente";
}
