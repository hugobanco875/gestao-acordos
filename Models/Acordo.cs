using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public class Acordo
{
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Selecione o cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    [StringLength(60)]
    [Display(Name = "Número do processo")]
    public string? NumeroProcesso { get; set; }

    [StringLength(200)]
    [Display(Name = "Descrição")]
    public string? Descricao { get; set; }

    [Range(0.01, 999999999, ErrorMessage = "Informe o valor total do acordo.")]
    [Display(Name = "Valor total")]
    public decimal ValorTotal { get; set; }

    [Range(1, 360, ErrorMessage = "Informe a quantidade de parcelas (1 a 360).")]
    [Display(Name = "Quantidade de parcelas")]
    public int QuantidadeParcelas { get; set; } = 1;

    [Display(Name = "Data da 1ª parcela")]
    public DateOnly DataPrimeiraParcela { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    // Metadados do PDF do acordo (o conteúdo fica em AcordoPdf).
    public string? PdfNomeArquivo { get; set; }
    public string? PdfContentType { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public List<Parcela> Parcelas { get; set; } = new();
    public AcordoPdf? Pdf { get; set; }

    public bool TemPdf => !string.IsNullOrEmpty(PdfNomeArquivo);
}
