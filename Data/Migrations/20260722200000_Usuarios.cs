using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260722200000_Usuarios")]
public partial class Usuarios : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(name: "Ativo", table: "AspNetUsers", type: "boolean", nullable: false, defaultValue: true);
        migrationBuilder.AddColumn<bool>(name: "Administrador", table: "AspNetUsers", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>(name: "SolicitouAdministrador", table: "AspNetUsers", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<DateTime>(name: "CriadoEm", table: "AspNetUsers", type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP");
        migrationBuilder.AddColumn<DateTime>(name: "UltimoAcessoEm", table: "AspNetUsers", type: "timestamp without time zone", nullable: true);
        migrationBuilder.AddColumn<string>(name: "AdministradorAutorizadorId", table: "AspNetUsers", type: "text", nullable: true);
        migrationBuilder.AddColumn<DateTime>(name: "AdministradorAutorizadoEm", table: "AspNetUsers", type: "timestamp without time zone", nullable: true);

        migrationBuilder.CreateIndex(name: "IX_AspNetUsers_Ativo", table: "AspNetUsers", column: "Ativo");
        migrationBuilder.CreateIndex(name: "IX_AspNetUsers_Administrador", table: "AspNetUsers", column: "Administrador");
        migrationBuilder.CreateIndex(name: "IX_AspNetUsers_SolicitouAdministrador", table: "AspNetUsers", column: "SolicitouAdministrador");

        migrationBuilder.Sql("""
            UPDATE "AspNetUsers"
            SET "Administrador" = TRUE
            WHERE "Id" = (SELECT "Id" FROM "AspNetUsers" ORDER BY "CriadoEm", "Id" LIMIT 1)
              AND NOT EXISTS (SELECT 1 FROM "AspNetUsers" WHERE "Administrador" = TRUE);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(name: "IX_AspNetUsers_Ativo", table: "AspNetUsers");
        migrationBuilder.DropIndex(name: "IX_AspNetUsers_Administrador", table: "AspNetUsers");
        migrationBuilder.DropIndex(name: "IX_AspNetUsers_SolicitouAdministrador", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "Ativo", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "Administrador", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "SolicitouAdministrador", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "CriadoEm", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "UltimoAcessoEm", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "AdministradorAutorizadorId", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "AdministradorAutorizadoEm", table: "AspNetUsers");
    }
}
