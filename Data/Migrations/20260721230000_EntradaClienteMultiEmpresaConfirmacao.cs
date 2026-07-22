using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260721230000_EntradaClienteMultiEmpresaConfirmacao")]
public partial class EntradaClienteMultiEmpresaConfirmacao : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "ValorEntrada",
            table: "Acordos",
            type: "numeric(18,2)",
            precision: 18,
            scale: 2,
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<DateOnly>(
            name: "DataEntrada",
            table: "Acordos",
            type: "date",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "EmpresaId",
            table: "Acordos",
            type: "integer",
            nullable: true);

        migrationBuilder.Sql("""
            UPDATE "Acordos" a
            SET "EmpresaId" = c."EmpresaId"
            FROM "Clientes" c
            WHERE a."ClienteId" = c."Id";
            """);

        migrationBuilder.AlterColumn<int>(
            name: "EmpresaId",
            table: "Acordos",
            type: "integer",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer",
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "ClienteEmpresas",
            columns: table => new
            {
                ClienteId = table.Column<int>(type: "integer", nullable: false),
                EmpresaId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClienteEmpresas", x => new { x.ClienteId, x.EmpresaId });
                table.ForeignKey("FK_ClienteEmpresas_Clientes_ClienteId", x => x.ClienteId, "Clientes", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_ClienteEmpresas_Empresas_EmpresaId", x => x.EmpresaId, "Empresas", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql("""
            INSERT INTO "ClienteEmpresas" ("ClienteId", "EmpresaId")
            SELECT "Id", "EmpresaId" FROM "Clientes"
            ON CONFLICT DO NOTHING;
            """);

        migrationBuilder.CreateIndex(name: "IX_Acordos_EmpresaId", table: "Acordos", column: "EmpresaId");
        migrationBuilder.CreateIndex(name: "IX_ClienteEmpresas_EmpresaId", table: "ClienteEmpresas", column: "EmpresaId");
        migrationBuilder.AddForeignKey(
            name: "FK_Acordos_Empresas_EmpresaId",
            table: "Acordos",
            column: "EmpresaId",
            principalTable: "Empresas",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(name: "FK_Acordos_Empresas_EmpresaId", table: "Acordos");
        migrationBuilder.DropTable(name: "ClienteEmpresas");
        migrationBuilder.DropIndex(name: "IX_Acordos_EmpresaId", table: "Acordos");
        migrationBuilder.DropColumn(name: "EmpresaId", table: "Acordos");
        migrationBuilder.DropColumn(name: "ValorEntrada", table: "Acordos");
        migrationBuilder.DropColumn(name: "DataEntrada", table: "Acordos");
    }
}
