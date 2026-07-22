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

    [StringLength(150)]
    [Display(Name = "Nome fantasia")]
    public string? NomeFantasia { get; set; }

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

    public bool Ativo { get; set; } = true;

    public DateTime CriadoEm { get; set; } = GestaoAcordos.Services.RelogioSistema.Agora;

    public List<Cliente> Clientes { get; set; } = new();
    public List<ClienteEmpresa> ClienteEmpresas { get; set; } = new();
    public List<Cliente> ClientesVinculados { get; set; } = new();
}
