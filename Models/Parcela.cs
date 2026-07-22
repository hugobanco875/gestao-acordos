using System.ComponentModel.DataAnnotations;

namespace GestaoAcordos.Models;

public enum TipoMovimentoFinanceiro
{
    Parcela = 0,
    Entrada = 1
}

public enum FormaPagamento
{
    NaoInformada = 0,
    Pix = 1,
    Dinheiro = 2,
    Boleto = 3,
    Transferencia = 4,
    CartaoCredito = 5,
    CartaoDebito = 6,
    Cheque = 7,
    Outro = 8,
    Deposito = 9
}

public class Parcela
{
    public int Id { get; set; }
    public int AcordoId { get; set; }
    public Acordo? Acordo { get; set; }

    public TipoMovimentoFinanceiro Tipo { get; set; } = TipoMovimentoFinanceiro.Parcela;

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

    [Display(Name = "Forma de pagamento")]
    public FormaPagamento FormaPagamento { get; set; }

    [StringLength(300)]
    [Display(Name = "Observação")]
    public string? Observacao { get; set; }

    public string? ComprovanteNomeArquivo { get; set; }
    public string? ComprovanteContentType { get; set; }
    public ParcelaComprovante? Comprovante { get; set; }

    public string DescricaoMovimento => Tipo == TipoMovimentoFinanceiro.Entrada ? "Entrada" : $"Parcela {Numero:000}";
}
