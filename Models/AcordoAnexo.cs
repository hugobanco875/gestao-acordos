namespace GestaoAcordos.Models;

/// <summary>
/// Arquivo anexado ao acordo. Um acordo pode possuir vários PDFs.
/// O conteúdo fica separado da entidade principal para não pesar as listagens.
/// </summary>
public class AcordoAnexo
{
    public int Id { get; set; }
    public int AcordoId { get; set; }
    public Acordo? Acordo { get; set; }

    public string NomeArquivo { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/pdf";
    public byte[] Conteudo { get; set; } = Array.Empty<byte>();
    public long TamanhoBytes { get; set; }
    public DateTime EnviadoEm { get; set; } = GestaoAcordos.Services.RelogioSistema.Agora;
}
