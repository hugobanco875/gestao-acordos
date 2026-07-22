using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

/// <summary>Configurações visuais do sistema (linha única, Id = 1).</summary>
public class Configuracao
{
    public int Id { get; set; }

    [StringLength(60)]
    [Display(Name = "Nome do sistema")]
    public string NomeSistema { get; set; } = "Gestão de Acordos";

    [StringLength(80)]
    [Display(Name = "Descrição da empresa")]
    public string SubtituloSistema { get; set; } = "Advocacia especializada";

    [StringLength(12)]
    [Display(Name = "Tema")]
    public string Tema { get; set; } = "claro";

    /// <summary>Logomarca personalizada em formato Data URL (PNG, JPEG ou WebP).</summary>
    public string? LogoDataUrl { get; set; }

    /// <summary>Cor de destaque (botões, links, cabeçalhos).</summary>
    [StringLength(9)]
    [Display(Name = "Cor principal")]
    public string CorPrimaria { get; set; } = "#0d6efd";

    /// <summary>Cor de fundo do menu lateral.</summary>
    [StringLength(9)]
    [Display(Name = "Cor do menu")]
    public string CorMenu { get; set; } = "#1b2a4a";
}
