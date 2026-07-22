using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestaoAcordos.Services;

public record RelatorioParcelaItem(
    string Empresa,
    string Cliente,
    string? NumeroProcesso,
    string Movimento,
    string FormaPagamento,
    decimal Valor,
    DateOnly Vencimento,
    DateOnly? DataPagamento,
    decimal? ValorPago,
    bool Paga);

public class RelatorioService
{
    private static string Situacao(RelatorioParcelaItem i, DateOnly hoje)
        => i.Paga ? "Recebida" : (i.Vencimento < hoje ? "Vencida" : "Em aberto");

    public byte[] GerarExcel(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
        var hoje = GestaoAcordos.Services.RelogioSistema.Hoje;
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Financeiro");
        ws.Cell(1, 1).Value = "Relatório Financeiro";
        ws.Range(1, 1, 1, 10).Merge().Style.Font.SetBold().Font.SetFontSize(14);
        ws.Cell(2, 1).Value = $"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}   |   {filtro}";
        ws.Range(2, 1, 2, 10).Merge().Style.Font.SetItalic();

        var h = 4;
        string[] cabecalhos = { "Empresa", "Cliente", "Processo", "Movimento", "Data", "Situação", "Pagamento", "Forma", "Valor previsto", "Valor recebido" };
        for (var c = 0; c < cabecalhos.Length; c++)
        {
            var cell = ws.Cell(h, c + 1); cell.Value = cabecalhos[c]; cell.Style.Font.SetBold();
            cell.Style.Fill.SetBackgroundColor(XLColor.FromHtml("#0d6efd")); cell.Style.Font.SetFontColor(XLColor.White);
        }
        var r = h + 1;
        foreach (var i in itens)
        {
            ws.Cell(r,1).Value=i.Empresa; ws.Cell(r,2).Value=i.Cliente; ws.Cell(r,3).Value=i.NumeroProcesso??""; ws.Cell(r,4).Value=i.Movimento;
            ws.Cell(r,5).Value=i.Vencimento.ToDateTime(TimeOnly.MinValue); ws.Cell(r,5).Style.DateFormat.Format="dd/MM/yyyy"; ws.Cell(r,6).Value=Situacao(i,hoje);
            if(i.DataPagamento is { } dp){ws.Cell(r,7).Value=dp.ToDateTime(TimeOnly.MinValue);ws.Cell(r,7).Style.DateFormat.Format="dd/MM/yyyy";}
            ws.Cell(r,8).Value=i.FormaPagamento; ws.Cell(r,9).Value=i.Valor; ws.Cell(r,10).Value=i.Paga?(i.ValorPago??i.Valor):0;
            ws.Cell(r,9).Style.NumberFormat.Format=ws.Cell(r,10).Style.NumberFormat.Format="\"R$\" #,##0.00"; r++;
        }
        var recebido=itens.Where(i=>i.Paga).Sum(i=>i.ValorPago??i.Valor); var aberto=itens.Where(i=>!i.Paga).Sum(i=>i.Valor);
        ws.Cell(r,9).Value="Recebido";ws.Cell(r,10).Value=recebido;ws.Cell(r+1,9).Value="Em aberto";ws.Cell(r+1,10).Value=aberto;
        ws.Range(r,9,r+1,10).Style.Font.SetBold();ws.Range(r,10,r+1,10).Style.NumberFormat.Format="\"R$\" #,##0.00";
        ws.Columns().AdjustToContents(); using var ms=new MemoryStream();wb.SaveAs(ms);return ms.ToArray();
    }

    public byte[] GerarPdf(IReadOnlyList<RelatorioParcelaItem> itens, DateOnly inicio, DateOnly fim, string filtro)
    {
        var hoje=GestaoAcordos.Services.RelogioSistema.Hoje;var recebido=itens.Where(i=>i.Paga).Sum(i=>i.ValorPago??i.Valor);var aberto=itens.Where(i=>!i.Paga).Sum(i=>i.Valor);
        return Document.Create(container=>container.Page(page=>
        {
            page.Size(PageSizes.A4.Landscape());page.Margin(22);page.DefaultTextStyle(t=>t.FontSize(8));
            page.Header().Column(c=>{c.Item().Text("Relatório Financeiro").FontSize(16).Bold().FontColor(Colors.Blue.Darken2);c.Item().Text($"Período: {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy} | {filtro}");});
            page.Content().PaddingTop(10).Table(table=>
            {
                table.ColumnsDefinition(c=>{c.RelativeColumn(2);c.RelativeColumn(2);c.RelativeColumn(2);c.RelativeColumn(1.5f);c.ConstantColumn(58);c.ConstantColumn(58);c.RelativeColumn(1.5f);c.ConstantColumn(70);});
                table.Header(h=>{void H(string t)=>h.Cell().Background(Colors.Blue.Medium).Padding(4).Text(t).FontColor(Colors.White).Bold();H("Empresa");H("Cliente");H("Processo");H("Movimento");H("Data");H("Situação");H("Forma");H("Valor");});
                var alt=false;foreach(var i in itens){var bg=alt?Colors.Grey.Lighten4:Colors.White;alt=!alt;void C(string t)=>table.Cell().Background(bg).Padding(3).Text(t);C(i.Empresa);C(i.Cliente);C(i.NumeroProcesso??"-");C(i.Movimento);C(i.Vencimento.ToString("dd/MM/yyyy"));C(Situacao(i,hoje));C(i.FormaPagamento);C((i.ValorPago??i.Valor).ToString("C"));}
            });
            page.Footer().AlignRight().Column(c=>{c.Item().Text($"Recebido: {recebido:C}").FontColor(Colors.Green.Darken2);c.Item().Text($"Em aberto: {aberto:C}").FontColor(Colors.Red.Darken1);});
        })).GeneratePdf();
    }
}
