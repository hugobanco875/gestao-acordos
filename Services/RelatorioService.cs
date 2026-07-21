using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestaoAcordos.Services;

/// <summary>Uma linha do relatório de parcelas baixadas.</summary>
public record RelatorioParcelaItem(
    string Empresa,
    string Cliente,
    string? NumeroProcesso,
    int NumeroParcela,
    decimal Valor,
    DateOnly Vencimento,
    DateOnly? DataPagamento,
    decimal? ValorPago);

public class RelatorioService
{
    // ---------- EXCEL ----------
    public byte[] GerarExcel(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Parcelas baixadas");

        ws.Cell(1, 1).Value = "Relatório de Parcelas Baixadas";
        ws.Range(1, 1, 1, 8).Merge().Style.Font.SetBold().Font.SetFontSize(14);

        ws.Cell(2, 1).Value = $"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}   |   {filtro}";
        ws.Range(2, 1, 2, 8).Merge().Style.Font.SetItalic();

        var h = 4;
        string[] cabecalhos = { "Empresa", "Cliente", "Processo", "Parcela", "Vencimento", "Pagamento", "Valor", "Valor pago" };
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
            if (i.DataPagamento is { } dp)
            {
                ws.Cell(r, 6).Value = dp.ToDateTime(TimeOnly.MinValue);
                ws.Cell(r, 6).Style.DateFormat.Format = "dd/MM/yyyy";
            }
            ws.Cell(r, 7).Value = i.Valor;
            ws.Cell(r, 7).Style.NumberFormat.Format = "\"R$\" #,##0.00";
            ws.Cell(r, 8).Value = i.ValorPago ?? i.Valor;
            ws.Cell(r, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";
            r++;
        }

        // Total
        ws.Cell(r, 6).Value = "TOTAL";
        ws.Cell(r, 6).Style.Font.SetBold();
        ws.Cell(r, 7).FormulaA1 = itens.Count > 0 ? $"SUM(G{h + 1}:G{r - 1})" : "0";
        ws.Cell(r, 8).FormulaA1 = itens.Count > 0 ? $"SUM(H{h + 1}:H{r - 1})" : "0";
        ws.Range(r, 7, r, 8).Style.Font.SetBold();
        ws.Range(r, 7, r, 8).Style.NumberFormat.Format = "\"R$\" #,##0.00";

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    // ---------- PDF ----------
    public byte[] GerarPdf(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
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
                    col.Item().Text("Relatório de Parcelas Baixadas").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
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
                        c.ConstantColumn(45); // parcela
                        c.ConstantColumn(65); // vencimento
                        c.ConstantColumn(65); // pagamento
                        c.ConstantColumn(75); // valor pago
                    });

                    table.Header(header =>
                    {
                        void H(string t) => header.Cell().Background(Colors.Blue.Medium).Padding(4)
                            .Text(t).FontColor(Colors.White).Bold();
                        H("Empresa"); H("Cliente"); H("Processo"); H("Parcela"); H("Vencimento"); H("Pagamento"); H("Valor pago");
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
                        C(i.DataPagamento?.ToString("dd/MM/yyyy") ?? "-");
                        C((i.ValorPago ?? i.Valor).ToString("C"));
                    }
                });

                page.Footer().PaddingTop(8).Row(row =>
                {
                    row.RelativeItem().Text($"{itens.Count} parcela(s)").FontSize(9);
                    row.ConstantItem(200).AlignRight().Text($"TOTAL: {total:C}").Bold().FontSize(11);
                });
            });
        });

        return doc.GeneratePdf();
    }
}
