using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;
using GestaoAcordos.Data;
using GestaoAcordos.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoAcordos.Services;

/// <summary>Estrutura do arquivo de backup (todos os dados do sistema, incluindo os PDFs).</summary>
public class BackupData
{
    public string GeradoEm { get; set; } = "";
    public List<Empresa> Empresas { get; set; } = new();
    public List<Cliente> Clientes { get; set; } = new();
    public List<ClienteEmpresa> ClienteEmpresas { get; set; } = new();
    public List<Acordo> Acordos { get; set; } = new();
    public List<AcordoPdf> AcordosPdf { get; set; } = new();
    public List<Parcela> Parcelas { get; set; } = new();
    public List<Evento> Eventos { get; set; } = new();
}

public class BackupService(
    IDbContextFactory<ApplicationDbContext> factory,
    AdministradorService administradorService)
{
    private static readonly JsonSerializerOptions _json = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    private async Task GarantirAdministradorAsync(ClaimsPrincipal principal)
    {
        if (!await administradorService.EhAdministradorAtivoAsync(principal))
        {
            throw new UnauthorizedAccessException(
                "Somente usuários administradores ativos podem gerar ou restaurar backups.");
        }
    }

    /// <summary>Gera um backup completo (JSON em bytes). Os PDFs vão embutidos em base64.</summary>
    public async Task<byte[]> ExportarAsync(ClaimsPrincipal principal)
    {
        await GarantirAdministradorAsync(principal);
        await using var db = await factory.CreateDbContextAsync();
        var data = new BackupData
        {
            GeradoEm = DateTime.Now.ToString("s"),
            Empresas = await db.Empresas.AsNoTracking().ToListAsync(),
            Clientes = await db.Clientes.AsNoTracking().ToListAsync(),
            ClienteEmpresas = await db.ClienteEmpresas.AsNoTracking().ToListAsync(),
            Acordos = await db.Acordos.AsNoTracking().ToListAsync(),
            AcordosPdf = await db.AcordosPdf.AsNoTracking().ToListAsync(),
            Parcelas = await db.Parcelas.AsNoTracking().ToListAsync(),
            Eventos = await db.Eventos.AsNoTracking().ToListAsync()
        };
        return JsonSerializer.SerializeToUtf8Bytes(data, _json);
    }

    /// <summary>
    /// Restaura um backup: APAGA os dados atuais (empresas/clientes/acordos/parcelas/eventos/PDFs)
    /// e reinsere os do arquivo. Os IDs são regerados e as ligações (FKs) remapeadas.
    /// </summary>
    public async Task<string> RestaurarAsync(byte[] jsonBytes, ClaimsPrincipal principal)
    {
        await GarantirAdministradorAsync(principal);
        var data = JsonSerializer.Deserialize<BackupData>(jsonBytes, _json)
                   ?? throw new InvalidOperationException("Arquivo de backup inválido.");

        await using var db = await factory.CreateDbContextAsync();
        await using var tx = await db.Database.BeginTransactionAsync();

        // Limpa os dados atuais (ordem segura de chaves estrangeiras).
        await db.Eventos.ExecuteDeleteAsync();
        await db.Parcelas.ExecuteDeleteAsync();
        await db.AcordosPdf.ExecuteDeleteAsync();
        await db.Acordos.ExecuteDeleteAsync();
        await db.ClienteEmpresas.ExecuteDeleteAsync();
        await db.Clientes.ExecuteDeleteAsync();
        await db.Empresas.ExecuteDeleteAsync();

        // Empresas
        var empMap = new Dictionary<int, int>();
        foreach (var e in data.Empresas)
        {
            var old = e.Id;
            e.Id = 0;
            e.Clientes = new();
            db.Empresas.Add(e);
            await db.SaveChangesAsync();
            empMap[old] = e.Id;
        }

        // Clientes (remapeia EmpresaId)
        var cliMap = new Dictionary<int, int>();
        foreach (var c in data.Clientes)
        {
            var old = c.Id;
            c.Id = 0;
            c.Acordos = new();
            c.Eventos = new();
            c.ClienteEmpresas = new();
            c.Empresas = new();
            c.Empresa = null;
            if (empMap.TryGetValue(c.EmpresaId, out var novaEmp)) c.EmpresaId = novaEmp;
            db.Clientes.Add(c);
            await db.SaveChangesAsync();
            cliMap[old] = c.Id;
        }

        // Vínculos entre clientes e empresas. Backups antigos usam apenas EmpresaId no cliente.
        var vinculos = data.ClienteEmpresas.Count > 0
            ? data.ClienteEmpresas
            : data.Clientes.Select(c => new ClienteEmpresa { ClienteId = c.Id, EmpresaId = c.EmpresaId }).ToList();
        foreach (var ce in vinculos)
        {
            if (!cliMap.TryGetValue(ce.ClienteId, out var novoCli)) continue;
            if (!empMap.TryGetValue(ce.EmpresaId, out var novaEmp)) continue;
            db.ClienteEmpresas.Add(new ClienteEmpresa { ClienteId = novoCli, EmpresaId = novaEmp });
        }
        await db.SaveChangesAsync();

        // Acordos (remapeia ClienteId)
        var acoMap = new Dictionary<int, int>();
        foreach (var a in data.Acordos)
        {
            var old = a.Id;
            var oldClienteId = a.ClienteId;
            a.Id = 0;
            a.Parcelas = new();
            a.Pdf = null;
            a.Cliente = null;
            a.Empresa = null;
            if (cliMap.TryGetValue(oldClienteId, out var novoCli)) a.ClienteId = novoCli;
            if (empMap.TryGetValue(a.EmpresaId, out var novaEmpAcordo))
                a.EmpresaId = novaEmpAcordo;
            else
            {
                var empresaAntiga = data.Clientes.FirstOrDefault(c => c.Id == oldClienteId)?.EmpresaId ?? 0;
                if (empMap.TryGetValue(empresaAntiga, out var empresaPrincipal)) a.EmpresaId = empresaPrincipal;
            }
            db.Acordos.Add(a);
            await db.SaveChangesAsync();
            acoMap[old] = a.Id;
        }

        // PDFs (PK = AcordoId)
        foreach (var p in data.AcordosPdf)
        {
            if (!acoMap.TryGetValue(p.AcordoId, out var novoAco)) continue;
            p.AcordoId = novoAco;
            p.Acordo = null;
            db.AcordosPdf.Add(p);
        }
        await db.SaveChangesAsync();

        // Parcelas (remapeia AcordoId)
        foreach (var p in data.Parcelas)
        {
            if (!acoMap.TryGetValue(p.AcordoId, out var novoAco)) continue;
            p.Id = 0;
            p.Acordo = null;
            p.AcordoId = novoAco;
            db.Parcelas.Add(p);
        }
        await db.SaveChangesAsync();

        // Eventos (remapeia Empresa/Cliente opcionais)
        foreach (var ev in data.Eventos)
        {
            ev.Id = 0;
            ev.Empresa = null;
            ev.Cliente = null;
            ev.EmpresaId = ev.EmpresaId is int ei && empMap.TryGetValue(ei, out var ne) ? ne : null;
            ev.ClienteId = ev.ClienteId is int ci && cliMap.TryGetValue(ci, out var nc) ? nc : null;
            db.Eventos.Add(ev);
        }
        await db.SaveChangesAsync();

        await tx.CommitAsync();

        return $"{data.Empresas.Count} empresa(s), {data.Clientes.Count} cliente(s), " +
               $"{data.Acordos.Count} acordo(s), {data.Parcelas.Count} parcela(s), " +
               $"{data.Eventos.Count} evento(s) e {data.AcordosPdf.Count} PDF(s).";
    }
}
