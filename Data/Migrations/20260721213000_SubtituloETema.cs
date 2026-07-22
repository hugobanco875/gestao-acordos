using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260721213000_SubtituloETema")]
public partial class SubtituloETema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "SubtituloSistema",
            table: "Configuracoes",
            type: "character varying(80)",
            maxLength: 80,
            nullable: false,
            defaultValue: "Advocacia especializada");

        migrationBuilder.AddColumn<string>(
            name: "Tema",
            table: "Configuracoes",
            type: "character varying(12)",
            maxLength: 12,
            nullable: false,
            defaultValue: "automatico");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "SubtituloSistema", table: "Configuracoes");
        migrationBuilder.DropColumn(name: "Tema", table: "Configuracoes");
    }
}
