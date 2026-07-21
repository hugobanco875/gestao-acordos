using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public class Parcela
{
    public int Id { get; set; }

    public int AcordoId { get; set; }
    public Acordo? Acordo { get; set; }

    [Display(Name = "Parcela")]
    public int Numero { get; set; }

    [Display(Name = "Valor")]
    public decimal Valor { get; set; }

    [Display(Name = "Vencimento")]
    public DateOnly DataVencimento { get; set; }

    [Display(Name = "Paga")]
    public bool Paga { get; set; }

    [Display(Name = "Data do pagamento")]
    public DateOnly? DataPagamento { get; set; }

    [Display(Name = "Valor pago")]
    public decimal? ValorPago { get; set; }

    [StringLength(300)]
    [Display(Name = "Observação")]
    public string? Observacao { get; set; }
}
