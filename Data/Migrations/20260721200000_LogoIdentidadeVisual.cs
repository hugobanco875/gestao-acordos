using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260721200000_LogoIdentidadeVisual")]
public partial class LogoIdentidadeVisual : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "LogoDataUrl",
            table: "Configuracoes",
            type: "text",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LogoDataUrl",
            table: "Configuracoes");
    }
}
