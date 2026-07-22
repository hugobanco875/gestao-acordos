using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public class Acordo
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione a empresa do acordo.")]
    [Display(Name = "Empresa")]
    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    [StringLength(60)]
    [Display(Name = "Número do processo")]
    public string? NumeroProcesso { get; set; }

    [StringLength(200)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "Informe o valor total do acordo.")]
    [Display(Name = "Valor total")]
    public decimal ValorTotal { get; set; }

    [Range(0, 999999999, ErrorMessage = "Informe um valor de entrada válido.")]
    [Display(Name = "Valor da entrada")]
    public decimal ValorEntrada { get; set; }

    [Display(Name = "Data da entrada")]
    public DateOnly? DataEntrada { get; set; }

    public decimal ValorParcelado => Math.Max(0, ValorTotal - ValorEntrada);

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [Display(Name = "Forma de pagamento da entrada")]
    public FormaPagamento FormaPagamentoEntrada { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    [StringLength(300)]
    [Display(Name = "Observação da entrada")]
    public string? ObservacaoEntrada { get; set; }

    [Range(1, 360, ErrorMessage = "Informe a quantidade de parcelas (1 a 360).")]
    [Display(Name = "Quantidade de parcelas")]
    public int QuantidadeParcelas { get; set; } = 1;

    [Display(Name = "Data da 1ª parcela")]
    public DateOnly DataPrimeiraParcela { get; set; } = GestaoAcordos.Services.RelogioSistema.Hoje;

    // Metadados do PDF do acordo (o conteúdo fica em AcordoPdf).
    public string? PdfNomeArquivo { get; set; }
    public string? PdfContentType { get; set; }

    public DateTime CriadoEm { get; set; } = GestaoAcordos.Services.RelogioSistema.Agora;

    public List<Parcela> Parcelas { get; set; } = new();
    public AcordoPdf? Pdf { get; set; }
    public List<AcordoAnexo> Anexos { get; set; } = new();

    public bool TemPdf => !string.IsNullOrEmpty(PdfNomeArquivo);
}
