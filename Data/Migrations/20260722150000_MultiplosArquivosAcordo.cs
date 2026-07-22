using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoAcordos.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260722150000_MultiplosArquivosAcordo")]
public partial class MultiplosArquivosAcordo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "AcordoAnexos",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                AcordoId = table.Column<int>(type: "integer", nullable: false),
                NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                Conteudo = table.Column<byte[]>(type: "bytea", nullable: false),
                TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                EnviadoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AcordoAnexos", x => x.Id);
                table.ForeignKey("FK_AcordoAnexos_Acordos_AcordoId", x => x.AcordoId, "Acordos", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(name: "IX_AcordoAnexos_AcordoId", table: "AcordoAnexos", column: "AcordoId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable(name: "AcordoAnexos");
}
