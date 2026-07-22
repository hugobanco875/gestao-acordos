using GestaoAcordos.Models;

namespace GestaoAcordos.Services;

public static class FormasPagamento
{
    public static IReadOnlyList<FormaPagamento> OpcoesRecebimento { get; } =
    [
        FormaPagamento.Pix,
        FormaPagamento.Deposito,
        FormaPagamento.Dinheiro,
        FormaPagamento.Transferencia,
        FormaPagamento.Boleto,
        FormaPagamento.CartaoDebito,
        FormaPagamento.CartaoCredito,
        FormaPagamento.Cheque,
        FormaPagamento.Outro
    ];

    public static string Nome(FormaPagamento forma) => forma switch
    {
        FormaPagamento.NaoInformada => "Não informada",
        FormaPagamento.Pix => "PIX",
        FormaPagamento.Deposito => "Depósito",
        FormaPagamento.Dinheiro => "Dinheiro",
        FormaPagamento.Transferencia => "Transferência",
        FormaPagamento.Boleto => "Boleto",
        FormaPagamento.CartaoDebito => "Cartão de débito",
        FormaPagamento.CartaoCredito => "Cartão de crédito",
        FormaPagamento.Cheque => "Cheque",
        _ => "Outro"
    };

    public static string Icone(FormaPagamento forma) => forma switch
    {
        FormaPagamento.Pix => "◆",
        FormaPagamento.Deposito => "🏦",
        FormaPagamento.Dinheiro => "💵",
        FormaPagamento.Transferencia => "↔",
        FormaPagamento.Boleto => "🧾",
        FormaPagamento.CartaoDebito => "💳",
        FormaPagamento.CartaoCredito => "💳",
        FormaPagamento.Cheque => "📝",
        FormaPagamento.Outro => "•",
        _ => ""
    };

    public static string RotuloSelecao(FormaPagamento forma)
        => $"{Icone(forma)} {Nome(forma)}".Trim();
}
