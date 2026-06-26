using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutocleanManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoverDadosSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Agendamentos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Agendamentos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Agendamentos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposLavagem",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TiposLavagem",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TiposLavagem",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Veiculos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposLavagem",
                columns: new[] { "Id", "DuracaoEstimadaMinutos", "Nome", "PrecoBase" },
                values: new object[,]
                {
                    { 1, 30, "Lavagem externa", 35m },
                    { 2, 40, "Lavagem interna", 40m },
                    { 3, 60, "Lavagem completa", 60m }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Nome", "Papel" },
                values: new object[,]
                {
                    { 1, "joao@email.com", "Joao Silva", "Cliente" },
                    { 2, "ana@email.com", "Ana Martins", "Cliente" },
                    { 3, "carlos@email.com", "Carlos Lima", "Funcionario" }
                });

            migrationBuilder.InsertData(
                table: "Veiculos",
                columns: new[] { "Id", "Ano", "Cor", "Marca", "Modelo", "Placa", "UsuarioId" },
                values: new object[,]
                {
                    { 1, 2021, "Prata", "Fiat", "Argo", "ABC1D23", 1 },
                    { 2, 2020, "Preto", "Honda", "Civic", "HIJ7K89", 1 },
                    { 3, 2022, "Branco", "Volkswagen", "Polo", "EFG4H56", 2 },
                    { 4, 2023, "Cinza", "Toyota", "Corolla", "LMN0P12", 2 }
                });

            migrationBuilder.InsertData(
                table: "Agendamentos",
                columns: new[] { "Id", "DataHoraAgendada", "NivelSujeira", "PrecoTotal", "Status", "TipoLavagemId", "UsuarioId", "VeiculoId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 20, 18, 0, 0, 0, DateTimeKind.Utc), "Pesada", 72m, "Confirmado", 3, 1, 1 },
                    { 2, new DateTime(2026, 3, 21, 10, 0, 0, 0, DateTimeKind.Utc), "Leve", 35m, "Aguardando", 1, 1, 2 },
                    { 3, new DateTime(2026, 3, 22, 14, 0, 0, 0, DateTimeKind.Utc), "Media", 44m, "Na fila", 2, 2, 3 }
                });
        }
    }
}
