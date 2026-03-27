using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AutocleanManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TiposLavagem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    BasePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    EstimatedDurationMinutes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposLavagem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Plate = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Veiculos_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    WashTypeId = table.Column<int>(type: "integer", nullable: false),
                    DirtLevel = table.Column<string>(type: "text", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendamentos_TiposLavagem_WashTypeId",
                        column: x => x.WashTypeId,
                        principalTable: "TiposLavagem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Veiculos_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TiposLavagem",
                columns: new[] { "Id", "BasePrice", "EstimatedDurationMinutes", "Name" },
                values: new object[,]
                {
                    { 1, 35m, 30, "Lavagem externa" },
                    { 2, 40m, 40, "Lavagem interna" },
                    { 3, 60m, 60, "Lavagem completa" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Email", "Name", "Role" },
                values: new object[,]
                {
                    { 1, "joao@email.com", "Joao Silva", "Cliente" },
                    { 2, "ana@email.com", "Ana Martins", "Cliente" },
                    { 3, "carlos@email.com", "Carlos Lima", "Funcionario" }
                });

            migrationBuilder.InsertData(
                table: "Veiculos",
                columns: new[] { "Id", "Brand", "Color", "Model", "Plate", "UserId", "Year" },
                values: new object[,]
                {
                    { 1, "Fiat", "Prata", "Argo", "ABC1D23", 1, 2021 },
                    { 2, "Honda", "Preto", "Civic", "HIJ7K89", 1, 2020 },
                    { 3, "Volkswagen", "Branco", "Polo", "EFG4H56", 2, 2022 },
                    { 4, "Toyota", "Cinza", "Corolla", "LMN0P12", 2, 2023 }
                });

            migrationBuilder.InsertData(
                table: "Agendamentos",
                columns: new[] { "Id", "DirtLevel", "ScheduledAt", "Status", "TotalPrice", "UserId", "VehicleId", "WashTypeId" },
                values: new object[,]
                {
                    { 1, "Pesada", new DateTime(2026, 3, 20, 18, 0, 0, 0, DateTimeKind.Utc), "Confirmado", 72m, 1, 1, 3 },
                    { 2, "Leve", new DateTime(2026, 3, 21, 10, 0, 0, 0, DateTimeKind.Utc), "Aguardando", 35m, 1, 2, 1 },
                    { 3, "Media", new DateTime(2026, 3, 22, 14, 0, 0, 0, DateTimeKind.Utc), "Na fila", 44m, 2, 3, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_UserId",
                table: "Agendamentos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_VehicleId",
                table: "Agendamentos",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_WashTypeId",
                table: "Agendamentos",
                column: "WashTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Veiculos_UserId",
                table: "Veiculos",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "TiposLavagem");

            migrationBuilder.DropTable(
                name: "Veiculos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
