using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutocleanManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenomearColunasParaPortugues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_TiposLavagem_WashTypeId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Usuarios_UserId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Veiculos_VehicleId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Usuarios_UserId",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Veiculos",
                newName: "Ano");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Veiculos",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "Plate",
                table: "Veiculos",
                newName: "Placa");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "Veiculos",
                newName: "Modelo");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "Veiculos",
                newName: "Cor");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "Veiculos",
                newName: "Marca");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_UserId",
                table: "Veiculos",
                newName: "IX_Veiculos_UsuarioId");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "Usuarios",
                newName: "Papel");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Usuarios",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "TiposLavagem",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "EstimatedDurationMinutes",
                table: "TiposLavagem",
                newName: "DuracaoEstimadaMinutos");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "TiposLavagem",
                newName: "PrecoBase");

            migrationBuilder.RenameColumn(
                name: "WashTypeId",
                table: "Agendamentos",
                newName: "TipoLavagemId");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Agendamentos",
                newName: "VeiculoId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Agendamentos",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Agendamentos",
                newName: "PrecoTotal");

            migrationBuilder.RenameColumn(
                name: "ScheduledAt",
                table: "Agendamentos",
                newName: "DataHoraAgendada");

            migrationBuilder.RenameColumn(
                name: "DirtLevel",
                table: "Agendamentos",
                newName: "NivelSujeira");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_WashTypeId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_TipoLavagemId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_VehicleId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_VeiculoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_UserId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_TiposLavagem_TipoLavagemId",
                table: "Agendamentos",
                column: "TipoLavagemId",
                principalTable: "TiposLavagem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Usuarios_UsuarioId",
                table: "Agendamentos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Veiculos_VeiculoId",
                table: "Agendamentos",
                column: "VeiculoId",
                principalTable: "Veiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Usuarios_UsuarioId",
                table: "Veiculos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_TiposLavagem_TipoLavagemId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Usuarios_UsuarioId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Veiculos_VeiculoId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Veiculos_Usuarios_UsuarioId",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Veiculos",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Placa",
                table: "Veiculos",
                newName: "Plate");

            migrationBuilder.RenameColumn(
                name: "Modelo",
                table: "Veiculos",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "Marca",
                table: "Veiculos",
                newName: "Brand");

            migrationBuilder.RenameColumn(
                name: "Cor",
                table: "Veiculos",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "Ano",
                table: "Veiculos",
                newName: "Year");

            migrationBuilder.RenameIndex(
                name: "IX_Veiculos_UsuarioId",
                table: "Veiculos",
                newName: "IX_Veiculos_UserId");

            migrationBuilder.RenameColumn(
                name: "Papel",
                table: "Usuarios",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Usuarios",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "PrecoBase",
                table: "TiposLavagem",
                newName: "BasePrice");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "TiposLavagem",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "DuracaoEstimadaMinutos",
                table: "TiposLavagem",
                newName: "EstimatedDurationMinutes");

            migrationBuilder.RenameColumn(
                name: "VeiculoId",
                table: "Agendamentos",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "Agendamentos",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TipoLavagemId",
                table: "Agendamentos",
                newName: "WashTypeId");

            migrationBuilder.RenameColumn(
                name: "PrecoTotal",
                table: "Agendamentos",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "NivelSujeira",
                table: "Agendamentos",
                newName: "DirtLevel");

            migrationBuilder.RenameColumn(
                name: "DataHoraAgendada",
                table: "Agendamentos",
                newName: "ScheduledAt");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_VeiculoId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_UsuarioId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_TipoLavagemId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_WashTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_TiposLavagem_WashTypeId",
                table: "Agendamentos",
                column: "WashTypeId",
                principalTable: "TiposLavagem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Usuarios_UserId",
                table: "Agendamentos",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Veiculos_VehicleId",
                table: "Agendamentos",
                column: "VehicleId",
                principalTable: "Veiculos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Veiculos_Usuarios_UserId",
                table: "Veiculos",
                column: "UserId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
