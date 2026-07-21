using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public class Cliente
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione a empresa.")]
    [Display(Name = "Empresa")]
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    [Required(ErrorMessage = "Informe o nome do cliente.")]
    [StringLength(150)]
    [Display(Name = "Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "CPF/CNPJ")]
    public string? CpfCnpj { get; set; }

    [StringLength(20)]
    [Display(Name = "Telefone")]
    public string? Telefone { get; set; }

    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [StringLength(150)]
    [Display(Name = "E-mail")]
    public string? Email { get; set; }

    [StringLength(10)]
    [Display(Name = "CEP")]
    public string? Cep { get; set; }

    [StringLength(200)]
    [Display(Name = "Endereço")]
    public string? Logradouro { get; set; }

    [StringLength(20)]
    [Display(Name = "Número")]
    public string? Numero { get; set; }

    [StringLength(100)]
    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }

    [StringLength(100)]
    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }

    [StringLength(100)]
    [Display(Name = "Cidade")]
    public string? Cidade { get; set; }

    [StringLength(2)]
    [Display(Name = "UF")]
    public string? Uf { get; set; }

    [StringLength(500)]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public List<Acordo> Acordos { get; set; } = new();
    public List<Evento> Eventos { get; set; } = new();
}
