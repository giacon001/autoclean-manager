using AutocleanManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AutocleanManager.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Veiculo> Veiculos { get; set; } = null!;
    public DbSet<TipoLavagem> TiposLavagem { get; set; } = null!;
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed de dados iniciais
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nome = "Joao Silva", Email = "joao@email.com", Papel = "Cliente" },
            new Usuario { Id = 2, Nome = "Ana Martins", Email = "ana@email.com", Papel = "Cliente" },
            new Usuario { Id = 3, Nome = "Carlos Lima", Email = "carlos@email.com", Papel = "Funcionario" }
        );

        modelBuilder.Entity<Veiculo>().HasData(
            new Veiculo { Id = 1, UsuarioId = 1, Marca = "Fiat", Modelo = "Argo", Placa = "ABC1D23", Cor = "Prata", Ano = 2021 },
            new Veiculo { Id = 2, UsuarioId = 1, Marca = "Honda", Modelo = "Civic", Placa = "HIJ7K89", Cor = "Preto", Ano = 2020 },
            new Veiculo { Id = 3, UsuarioId = 2, Marca = "Volkswagen", Modelo = "Polo", Placa = "EFG4H56", Cor = "Branco", Ano = 2022 },
            new Veiculo { Id = 4, UsuarioId = 2, Marca = "Toyota", Modelo = "Corolla", Placa = "LMN0P12", Cor = "Cinza", Ano = 2023 }
        );

        modelBuilder.Entity<TipoLavagem>().HasData(
            new TipoLavagem { Id = 1, Nome = "Lavagem externa", PrecoBase = 35m, DuracaoEstimadaMinutos = 30 },
            new TipoLavagem { Id = 2, Nome = "Lavagem interna", PrecoBase = 40m, DuracaoEstimadaMinutos = 40 },
            new TipoLavagem { Id = 3, Nome = "Lavagem completa", PrecoBase = 60m, DuracaoEstimadaMinutos = 60 }
        );

        modelBuilder.Entity<Agendamento>().HasData(
            new Agendamento
            {
                Id = 1,
                UsuarioId = 1,
                VeiculoId = 1,
                TipoLavagemId = 3,
                NivelSujeira = "Pesada",
                DataHoraAgendada = new DateTime(2026, 3, 20, 18, 0, 0, DateTimeKind.Utc),
                Status = "Confirmado",
                PrecoTotal = 72m
            },
            new Agendamento
            {
                Id = 2,
                UsuarioId = 1,
                VeiculoId = 2,
                TipoLavagemId = 1,
                NivelSujeira = "Leve",
                DataHoraAgendada = new DateTime(2026, 3, 21, 10, 0, 0, DateTimeKind.Utc),
                Status = "Aguardando",
                PrecoTotal = 35m
            },
            new Agendamento
            {
                Id = 3,
                UsuarioId = 2,
                VeiculoId = 3,
                TipoLavagemId = 2,
                NivelSujeira = "Media",
                DataHoraAgendada = new DateTime(2026, 3, 22, 14, 0, 0, DateTimeKind.Utc),
                Status = "Na fila",
                PrecoTotal = 44m
            }
        );

        // Relacionamentos
        modelBuilder.Entity<Veiculo>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(v => v.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Agendamento>()
            .HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(a => a.UsuarioId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Agendamento>()
            .HasOne<Veiculo>()
            .WithMany()
            .HasForeignKey(a => a.VeiculoId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Agendamento>()
            .HasOne<TipoLavagem>()
            .WithMany()
            .HasForeignKey(a => a.TipoLavagemId)
            .IsRequired();
    }
}
