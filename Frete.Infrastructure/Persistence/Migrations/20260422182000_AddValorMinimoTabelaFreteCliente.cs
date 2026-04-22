using Frete.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Frete.Infrastructure.Persistence.Migrations;

[DbContext(typeof(FreteDbContext))]
[Migration("20260422182000_AddValorMinimoTabelaFreteCliente")]
public class AddValorMinimoTabelaFreteCliente : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "ValorMinimo",
            table: "tabelas_frete_cliente",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ValorMinimo",
            table: "tabelas_frete_cliente");
    }
}
