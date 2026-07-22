using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260722003000_MovimentacoesFinanceiras")]
public partial class MovimentacoesFinanceiras : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(name: "Tipo", table: "Parcelas", type: "integer", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<int>(name: "FormaPagamento", table: "Parcelas", type: "integer", nullable: false, defaultValue: 0);
        migrationBuilder.AddColumn<string>(name: "ComprovanteNomeArquivo", table: "Parcelas", type: "character varying(255)", maxLength: 255, nullable: true);
        migrationBuilder.AddColumn<string>(name: "ComprovanteContentType", table: "Parcelas", type: "character varying(100)", maxLength: 100, nullable: true);
        migrationBuilder.CreateTable(
            name: "ParcelaComprovantes",
            columns: table => new
            {
                ParcelaId = table.Column<int>(type: "integer", nullable: false),
                Conteudo = table.Column<byte[]>(type: "bytea", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ParcelaComprovantes", x => x.ParcelaId);
                table.ForeignKey(name: "FK_ParcelaComprovantes_Parcelas_ParcelaId", column: x => x.ParcelaId, principalTable: "Parcelas", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
            });
        migrationBuilder.CreateIndex(name: "IX_Parcelas_Tipo", table: "Parcelas", column: "Tipo");

        migrationBuilder.Sql("""
            INSERT INTO "Parcelas" ("AcordoId", "Numero", "Valor", "DataVencimento", "Paga", "DataPagamento", "ValorPago", "Observacao", "Tipo", "FormaPagamento")
            SELECT a."Id", 0, a."ValorEntrada", COALESCE(a."DataEntrada", a."CriadoEm"::date), TRUE,
                   COALESCE(a."DataEntrada", a."CriadoEm"::date), a."ValorEntrada", 'Entrada migrada automaticamente', 1, 0
            FROM "Acordos" a
            WHERE a."ValorEntrada" > 0
              AND NOT EXISTS (SELECT 1 FROM "Parcelas" p WHERE p."AcordoId" = a."Id" AND p."Tipo" = 1);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ParcelaComprovantes");
        migrationBuilder.DropIndex(name: "IX_Parcelas_Tipo", table: "Parcelas");
        migrationBuilder.DropColumn(name: "Tipo", table: "Parcelas");
        migrationBuilder.DropColumn(name: "FormaPagamento", table: "Parcelas");
        migrationBuilder.DropColumn(name: "ComprovanteNomeArquivo", table: "Parcelas");
        migrationBuilder.DropColumn(name: "ComprovanteContentType", table: "Parcelas");
    }
}
