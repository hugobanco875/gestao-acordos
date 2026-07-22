using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public enum TipoEvento
{
    [Display(Name = "Reunião")]
    Reuniao = 1,

    [Display(Name = "Audiência")]
    Audiencia = 2,

    [Display(Name = "Outro")]
    Outro = 99
}

public class Evento
{
    public int Id { get; set; }

    [Display(Name = "Tipo")]
    public TipoEvento Tipo { get; set; } = TipoEvento.Reuniao;

    [Required(ErrorMessage = "Informe o título/assunto.")]
    [StringLength(150)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Display(Name = "Data e hora")]
    public DateTime DataHora { get; set; } = GestaoAcordos.Services.RelogioSistema.Agora;

    [Display(Name = "Empresa")]
    public int? EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    [Display(Name = "Cliente")]
    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [StringLength(150)]
    [Display(Name = "Local")]
    public string? Local { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    [Display(Name = "Concluído")]
    public bool Concluido { get; set; }

    public DateTime CriadoEm { get; set; } = GestaoAcordos.Services.RelogioSistema.Agora;
}
