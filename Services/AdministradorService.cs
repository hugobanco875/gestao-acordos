using System.Security.Claims;
using GestaoAcordos.Data;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Services;

/// <summary>
/// Centraliza as regras de autorização administrativa do sistema.
/// A validação é feita diretamente no banco pelo identificador da sessão,
/// evitando inconsistências do UserManager dentro do circuito do Blazor.
/// </summary>
public sealed class AdministradorService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public AdministradorService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<ApplicationUser?> ObterUsuarioAtualAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var usuarioId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? principal.FindFirstValue("sub");

        await using var db = await _dbFactory.CreateDbContextAsync();

        if (!string.IsNullOrWhiteSpace(usuarioId))
        {
            var usuarioPorId = await db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == usuarioId);

            if (usuarioPorId is not null)
            {
                return usuarioPorId;
            }
        }

        // Compatibilidade com sessões antigas que eventualmente não possuam NameIdentifier.
        var nome = principal.Identity?.Name;
        if (string.IsNullOrWhiteSpace(nome))
        {
            return null;
        }

        var nomeNormalizado = nome.Trim().ToUpperInvariant();
        return await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.NormalizedUserName == nomeNormalizado ||
                x.NormalizedEmail == nomeNormalizado);
    }

    public async Task<ApplicationUser?> ObterAdministradorAtivoAsync(ClaimsPrincipal principal)
    {
        var usuario = await ObterUsuarioAtualAsync(principal);
        return usuario is { Ativo: true, Administrador: true } ? usuario : null;
    }

    public async Task<bool> EhAdministradorAtivoAsync(ClaimsPrincipal principal) =>
        await ObterAdministradorAtivoAsync(principal) is not null;

    public async Task<bool> ExisteUsuarioAsync()
    {
        await using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Users.AsNoTracking().AnyAsync();
    }
}
