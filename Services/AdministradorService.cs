using System.Security.Claims;
using GestaoAcordos.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Services;

/// <summary>
/// Centraliza as regras de autorização administrativa do sistema.
/// Um administrador válido precisa existir, estar ativo e possuir o perfil administrativo.
/// </summary>
public sealed class AdministradorService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public AdministradorService(
        UserManager<ApplicationUser> userManager,
        IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _userManager = userManager;
        _dbFactory = dbFactory;
    }

    public async Task<ApplicationUser?> ObterAdministradorAtivoAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var usuario = await _userManager.GetUserAsync(principal);
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
