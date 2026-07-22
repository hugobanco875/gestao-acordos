using GestaoAcordos.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ClienteEmpresa> ClienteEmpresas => Set<ClienteEmpresa>();
    public DbSet<Acordo> Acordos => Set<Acordo>();
    public DbSet<AcordoPdf> AcordosPdf => Set<AcordoPdf>();
    public DbSet<AcordoAnexo> AcordoAnexos => Set<AcordoAnexo>();
    public DbSet<Parcela> Parcelas => Set<Parcela>();
    public DbSet<ParcelaComprovante> ParcelaComprovantes => Set<ParcelaComprovante>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Configuracao> Configuracoes => Set<Configuracao>();

    // Chaves de proteção de dados (cookies/antiforgery) persistidas no banco,
    // para sobreviverem a reinícios/deploys (senão o logout dá erro no Render).
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<ApplicationUser>(e =>
        {
            e.HasIndex(x => x.Ativo);
            e.HasIndex(x => x.Administrador);
            e.HasIndex(x => x.SolicitouAdministrador);
        });

        builder.Entity<Empresa>(e =>
        {
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.HasIndex(x => x.Nome);
        });

        builder.Entity<Cliente>(e =>
        {
            e.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            e.HasOne(x => x.Empresa)
                .WithMany(x => x.Clientes)
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Empresas)
                .WithMany(x => x.ClientesVinculados)
                .UsingEntity<ClienteEmpresa>(
                    right => right.HasOne(x => x.Empresa)
                        .WithMany(x => x.ClienteEmpresas)
                        .HasForeignKey(x => x.EmpresaId)
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne(x => x.Cliente)
                        .WithMany(x => x.ClienteEmpresas)
                        .HasForeignKey(x => x.ClienteId)
                        .OnDelete(DeleteBehavior.Cascade),
                    join => join.HasKey(x => new { x.ClienteId, x.EmpresaId }));
            e.HasIndex(x => x.Nome);
        });

        builder.Entity<Acordo>(e =>
        {
            e.Property(x => x.ValorTotal).HasPrecision(18, 2);
            e.Property(x => x.ValorEntrada).HasPrecision(18, 2);
            e.HasOne(x => x.Cliente)
                .WithMany(x => x.Acordos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Empresa)
                .WithMany()
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<AcordoPdf>(e =>
        {
            e.HasKey(x => x.AcordoId);
            e.HasOne(x => x.Acordo)
                .WithOne(x => x.Pdf)
                .HasForeignKey<AcordoPdf>(x => x.AcordoId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        builder.Entity<AcordoAnexo>(e =>
        {
            e.Property(x => x.NomeArquivo).IsRequired().HasMaxLength(255);
            e.Property(x => x.ContentType).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Acordo)
                .WithMany(x => x.Anexos)
                .HasForeignKey(x => x.AcordoId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.AcordoId);
        });

        builder.Entity<Parcela>(e =>
        {
            e.Property(x => x.Valor).HasPrecision(18, 2);
            e.Property(x => x.ValorPago).HasPrecision(18, 2);
            e.Property(x => x.ComprovanteNomeArquivo).HasMaxLength(255);
            e.Property(x => x.ComprovanteContentType).HasMaxLength(100);
            e.HasOne(x => x.Acordo)
                .WithMany(x => x.Parcelas)
                .HasForeignKey(x => x.AcordoId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.DataVencimento);
            e.HasIndex(x => x.Paga);
            e.HasIndex(x => x.Tipo);
        });

        builder.Entity<ParcelaComprovante>(e =>
        {
            e.HasKey(x => x.ParcelaId);
            e.HasOne(x => x.Parcela)
                .WithOne(x => x.Comprovante)
                .HasForeignKey<ParcelaComprovante>(x => x.ParcelaId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Evento>(e =>
        {
            e.Property(x => x.Titulo).IsRequired().HasMaxLength(150);
            e.HasOne(x => x.Empresa)
                .WithMany()
                .HasForeignKey(x => x.EmpresaId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Cliente)
                .WithMany(x => x.Eventos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => x.DataHora);
        });
    }

}