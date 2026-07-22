using Microsoft.AspNetCore.Identity;

namespace GestaoAcordos.Data;

public class ApplicationUser : IdentityUser
{
    public bool Ativo { get; set; } = true;
    public bool Administrador { get; set; }
    public bool SolicitouAdministrador { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public DateTime? UltimoAcessoEm { get; set; }
    public string? AdministradorAutorizadorId { get; set; }
    public DateTime? AdministradorAutorizadoEm { get; set; }
}
