using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frete.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialFretePersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tabelas_frete",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EmbarcadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabelas_frete", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tabelas_frete_cliente",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TabelaFreteId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalidadeOrigemId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocalidadeDestinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    VigenciaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    VigenciaFim = table.Column<DateOnly>(type: "date", nullable: true),
                    EmbarcadorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tabelas_frete_cliente", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tabelas_frete_cliente_tabelas_frete_TabelaFreteId",
                        column: x => x.TabelaFreteId,
                        principalTable: "tabelas_frete",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_EmbarcadorId",
                table: "tabelas_frete",
                column: "EmbarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_EmbarcadorId_Codigo",
                table: "tabelas_frete",
                columns: new[] { "EmbarcadorId", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_cliente_EmbarcadorId",
                table: "tabelas_frete_cliente",
                column: "EmbarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_cliente_EmbarcadorId_LocalidadeOrigemId_Local~",
                table: "tabelas_frete_cliente",
                columns: new[] { "EmbarcadorId", "LocalidadeOrigemId", "LocalidadeDestinoId" });

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_cliente_EmbarcadorId_TabelaFreteId",
                table: "tabelas_frete_cliente",
                columns: new[] { "EmbarcadorId", "TabelaFreteId" });

            migrationBuilder.CreateIndex(
                name: "IX_tabelas_frete_cliente_TabelaFreteId",
                table: "tabelas_frete_cliente",
                column: "TabelaFreteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tabelas_frete_cliente");

            migrationBuilder.DropTable(
                name: "tabelas_frete");
        }
    }
}
