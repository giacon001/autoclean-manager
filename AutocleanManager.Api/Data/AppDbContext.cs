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
