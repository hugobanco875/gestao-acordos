namespace GestaoAcordos.Models;

/// <summary>
/// Conteúdo binário do PDF do acordo, separado do <see cref="Acordo"/> (1:1)
/// para não carregar o arquivo em consultas de listagem.
/// </summary>
public class AcordoPdf
{
    public int AcordoId { get; set; }
    public Acordo? Acordo { get; set; }

    public byte[] Conteudo { get; set; } = Array.Empty<byte>();
    public long TamanhoBytes { get; set; }
    public DateTime EnviadoEm { get; set; } = DateTime.Now;
}
