using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public class Empresa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome da empresa.")]
    [StringLength(150)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "CNPJ/CPF")]
    public string? CnpjCpf { get; set; }

    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public List<Cliente> Clientes { get; set; } = new();
}
