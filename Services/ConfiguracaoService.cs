using GestaoAcordos.Data;
using GestaoAcordos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Services;

/// <summary>Lê e salva as configurações visuais (tema) do sistema — linha única (Id = 1).</summary>
public class ConfiguracaoService(IDbContextFactory<ApplicationDbContext> factory)
{
    public async Task<Configuracao> ObterAsync()
    {
        await using var db = await factory.CreateDbContextAsync();
        var cfg = await db.Configuracoes.AsNoTracking().OrderBy(c => c.Id).FirstOrDefaultAsync();
        return cfg ?? new Configuracao { Id = 1 };
    }

    public async Task SalvarAsync(Configuracao cfg)
    {
        await using var db = await factory.CreateDbContextAsync();
        var atual = await db.Configuracoes.OrderBy(c => c.Id).FirstOrDefaultAsync();
        if (atual is null)
        {
            cfg.Id = 1;
            db.Configuracoes.Add(cfg);
        }
        else
        {
            atual.NomeSistema = cfg.NomeSistema;
            atual.LogoDataUrl = cfg.LogoDataUrl;
            atual.SubtituloSistema = cfg.SubtituloSistema;
            atual.Tema = cfg.Tema;
            atual.CorPrimaria = cfg.CorPrimaria;
            atual.CorMenu = cfg.CorMenu;
        }
        await db.SaveChangesAsync();
    }
}
