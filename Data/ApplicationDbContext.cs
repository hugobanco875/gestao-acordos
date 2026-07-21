using GestaoAcordos.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Acordo> Acordos => Set<Acordo>();
    public DbSet<AcordoPdf> AcordosPdf => Set<AcordoPdf>();
    public DbSet<Parcela> Parcelas => Set<Parcela>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Configuracao> Configuracoes => Set<Configuracao>();

    // Chaves de proteção de dados (cookies/antiforgery) persistidas no banco,
    // para sobreviverem a reinícios/deploys (senão o logout dá erro no Render).
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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
            e.HasIndex(x => x.Nome);
        });

        builder.Entity<Acordo>(e =>
        {
            e.Property(x => x.ValorTotal).HasPrecision(18, 2);
            e.HasOne(x => x.Cliente)
                .WithMany(x => x.Acordos)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AcordoPdf>(e =>
        {
            e.HasKey(x => x.AcordoId);
            e.HasOne(x => x.Acordo)
                .WithOne(x => x.Pdf)
                .HasForeignKey<AcordoPdf>(x => x.AcordoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Parcela>(e =>
        {
            e.Property(x => x.Valor).HasPrecision(18, 2);
            e.Property(x => x.ValorPago).HasPrecision(18, 2);
            e.HasOne(x => x.Acordo)
                .WithMany(x => x.Parcelas)
                .HasForeignKey(x => x.AcordoId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.DataVencimento);
            e.HasIndex(x => x.Paga);
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
