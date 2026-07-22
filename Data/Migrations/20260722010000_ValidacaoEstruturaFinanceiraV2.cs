using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

/// <summary>
/// Validação idempotente da estrutura financeira. Esta migration funciona como
/// uma rede de segurança para instalações que receberam versões intermediárias
/// do projeto e ficaram com o banco parcialmente atualizado.
/// </summary>
[DbContext(typeof(ApplicationDbContext))]
[Migration("20260722010000_ValidacaoEstruturaFinanceiraV2")]
public partial class ValidacaoEstruturaFinanceiraV2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "Parcelas" ADD COLUMN IF NOT EXISTS "Tipo" integer NOT NULL DEFAULT 0;
            ALTER TABLE "Parcelas" ADD COLUMN IF NOT EXISTS "FormaPagamento" integer NOT NULL DEFAULT 0;
            ALTER TABLE "Parcelas" ADD COLUMN IF NOT EXISTS "ComprovanteNomeArquivo" character varying(255) NULL;
            ALTER TABLE "Parcelas" ADD COLUMN IF NOT EXISTS "ComprovanteContentType" character varying(100) NULL;

            CREATE TABLE IF NOT EXISTS "ParcelaComprovantes" (
                "ParcelaId" integer NOT NULL,
                "Conteudo" bytea NOT NULL,
                CONSTRAINT "PK_ParcelaComprovantes" PRIMARY KEY ("ParcelaId"),
                CONSTRAINT "FK_ParcelaComprovantes_Parcelas_ParcelaId"
                    FOREIGN KEY ("ParcelaId") REFERENCES "Parcelas" ("Id") ON DELETE CASCADE
            );

            CREATE INDEX IF NOT EXISTS "IX_Parcelas_Tipo" ON "Parcelas" ("Tipo");

            INSERT INTO "Parcelas"
                ("AcordoId", "Numero", "Valor", "DataVencimento", "Paga",
                 "DataPagamento", "ValorPago", "Observacao", "Tipo", "FormaPagamento")
            SELECT a."Id", 0, a."ValorEntrada",
                   COALESCE(a."DataEntrada", a."CriadoEm"::date), TRUE,
                   COALESCE(a."DataEntrada", a."CriadoEm"::date),
                   a."ValorEntrada", 'Entrada migrada automaticamente', 1, 0
              FROM "Acordos" a
             WHERE a."ValorEntrada" > 0
               AND NOT EXISTS (
                   SELECT 1
                     FROM "Parcelas" p
                    WHERE p."AcordoId" = a."Id" AND p."Tipo" = 1
               );
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Não remove dados nem colunas. Esta migration é apenas de validação e
        // recuperação de instalações parcialmente atualizadas.
    }
}
