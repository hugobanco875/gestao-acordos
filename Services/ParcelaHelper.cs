using GestaoAcordos.Models;

namespace GestaoAcordos.Services;

public static class ParcelaHelper
{
    /// <summary>
    /// Gera as parcelas de um acordo dividindo o valor total e distribuindo os
    /// vencimentos mês a mês a partir da data da primeira parcela.
    /// A última parcela absorve eventual diferença de arredondamento.
    /// </summary>
    public static List<Parcela> GerarParcelas(decimal valorTotal, int quantidade, DateOnly dataPrimeira)
    {
        if (quantidade < 1) quantidade = 1;

        var parcelas = new List<Parcela>();
        var valorBase = Math.Round(valorTotal / quantidade, 2, MidpointRounding.AwayFromZero);
        decimal acumulado = 0;

        for (int i = 0; i < quantidade; i++)
        {
            decimal valor = (i == quantidade - 1) ? valorTotal - acumulado : valorBase;
            acumulado += valor;

            parcelas.Add(new Parcela
            {
                Numero = i + 1,
                Valor = valor,
                DataVencimento = dataPrimeira.AddMonths(i),
                Paga = false
            });
        }

        return parcelas;
    }
}
