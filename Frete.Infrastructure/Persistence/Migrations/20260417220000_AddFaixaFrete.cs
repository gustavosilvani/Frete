using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frete.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class AddFaixaFrete : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "faixas_frete",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                TabelaFreteClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                LimiteInferiorKg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                LimiteSuperiorKg = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_faixas_frete", x => x.Id);
                table.ForeignKey(
                    name: "FK_faixas_frete_tabelas_frete_cliente_TabelaFreteClienteId",
                    column: x => x.TabelaFreteClienteId,
                    principalTable: "tabelas_frete_cliente",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_faixas_frete_TabelaFreteClienteId",
            table: "faixas_frete",
            column: "TabelaFreteClienteId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "faixas_frete");
    }
}
