using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestaoAcordos.Services;

/// <summary>Uma linha do relatório de parcelas.</summary>
public record RelatorioParcelaItem(
    string Empresa,
    string Cliente,
    string? NumeroProcesso,
    int NumeroParcela,
    decimal Valor,
    DateOnly Vencimento,
    DateOnly? DataPagamento,
    decimal? ValorPago,
    bool Paga);

public class RelatorioService
{
    private static string Situacao(RelatorioParcelaItem i, DateOnly hoje)
        => i.Paga ? "Paga" : (i.Vencimento < hoje ? "Vencida" : "Em aberto");

    // ---------- EXCEL ----------
    public byte[] GerarExcel(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Parcelas");

        ws.Cell(1, 1).Value = "Relatório de Parcelas";
        ws.Range(1, 1, 1, 8).Merge().Style.Font.SetBold().Font.SetFontSize(14);

        ws.Cell(2, 1).Value = $"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}   |   {filtro}";
        ws.Range(2, 1, 2, 8).Merge().Style.Font.SetItalic();

        var h = 4;
        string[] cabecalhos = { "Empresa", "Cliente", "Processo", "Parcela", "Vencimento", "Situação", "Pagamento", "Valor" };
        for (int c = 0; c < cabecalhos.Length; c++)
        {
            var cell = ws.Cell(h, c + 1);
            cell.Value = cabecalhos[c];
            cell.Style.Font.SetBold();
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0d6efd"));
            cell.Style.Font.SetFontColor(XLColor.White);
        }

        var r = h + 1;
        foreach (var i in itens)
        {
            ws.Cell(r, 1).Value = i.Empresa;
            ws.Cell(r, 2).Value = i.Cliente;
            ws.Cell(r, 3).Value = i.NumeroProcesso ?? "";
            ws.Cell(r, 4).Value = i.NumeroParcela;
            ws.Cell(r, 5).Value = i.Vencimento.ToDateTime(TimeOnly.MinValue);
            ws.Cell(r, 5).Style.DateFormat.Format = "dd/MM/yyyy";
            ws.Cell(r, 6).Value = Situacao(i, hoje);
            if (i.DataPagamento is { } dp)
            {
                ws.Cell(r, 7).Value = dp.ToDateTime(TimeOnly.MinValue);
                ws.Cell(r, 7).Style.DateFormat.Format = "dd/MM/yyyy";
            }
            ws.Cell(r, 8).Value = i.ValorPago ?? i.Valor;
            ws.Cell(r, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";
            r++;
        }

        // Totais
        ws.Cell(r, 7).Value = "TOTAL";
        ws.Cell(r, 7).Style.Font.SetBold();
        ws.Cell(r, 8).FormulaA1 = itens.Count > 0 ? $"SUM(H{h + 1}:H{r - 1})" : "0";
        ws.Cell(r, 8).Style.Font.SetBold();
        ws.Cell(r, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";

        var recebido = itens.Where(i => i.Paga).Sum(i => i.ValorPago ?? i.Valor);
        var aberto = itens.Where(i => !i.Paga).Sum(i => i.Valor);
        ws.Cell(r + 1, 7).Value = "Recebido";
        ws.Cell(r + 1, 8).Value = recebido;
        ws.Cell(r + 1, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";
        ws.Cell(r + 2, 7).Value = "Em aberto";
        ws.Cell(r + 2, 8).Value = aberto;
        ws.Cell(r + 2, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ---------- PDF ----------
    public byte[] GerarPdf(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var recebido = itens.Where(i => i.Paga).Sum(i => i.ValorPago ?? i.Valor);
        var aberto = itens.Where(i => !i.Paga).Sum(i => i.Valor);
        var total = itens.Sum(i => i.ValorPago ?? i.Valor);

        var doc = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(25);
                page.DefaultTextStyle(t => t.FontSize(9));

                page.Header().Column(col =>
                {
                    col.Item().Text("Relatório de Parcelas").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                    col.Item().Text($"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}").FontSize(9);
                    col.Item().Text(filtro).FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                });

                page.Content().PaddingTop(10).Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3); // empresa
                        c.RelativeColumn(3); // cliente
                        c.RelativeColumn(2); // processo
                        c.ConstantColumn(40); // parcela
                        c.ConstantColumn(62); // vencimento
                        c.ConstantColumn(60); // situacao
                        c.ConstantColumn(62); // pagamento
                        c.ConstantColumn(70); // valor
                    });

                    table.Header(header =>
                    {
                        void H(string t) => header.Cell().Background(Colors.Blue.Medium).Padding(4)
                            .Text(t).FontColor(Colors.White).Bold();
                        H("Empresa"); H("Cliente"); H("Processo"); H("Parcela"); H("Vencimento"); H("Situação"); H("Pagamento"); H("Valor");
                    });

                    var alt = false;
                    foreach (var i in itens)
                    {
                        var bg = alt ? Colors.Grey.Lighten4 : Colors.White;
                        alt = !alt;
                        void C(string t) => table.Cell().Background(bg).Padding(3).Text(t);
                        C(i.Empresa);
                        C(i.Cliente);
                        C(i.NumeroProcesso ?? "-");
                        C(i.NumeroParcela.ToString());
                        C(i.Vencimento.ToString("dd/MM/yyyy"));
                        C(Situacao(i, hoje));
                        C(i.DataPagamento?.ToString("dd/MM/yyyy") ?? "-");
                        C((i.ValorPago ?? i.Valor).ToString("C"));
                    }
                });

                page.Footer().PaddingTop(8).Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"{itens.Count} parcela(s)").FontSize(9);
                        row.ConstantItem(220).AlignRight().Text($"Recebido: {recebido:C}").FontSize(9).FontColor(Colors.Green.Darken2);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("");
                        row.ConstantItem(220).AlignRight().Text($"Em aberto: {aberto:C}").FontSize(9).FontColor(Colors.Red.Darken1);
                    });
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("");
                        row.ConstantItem(220).AlignRight().Text($"TOTAL: {total:C}").Bold().FontSize(11);
                    });
                });
            });
        });

        return doc.GeneratePdf();
    }
}
