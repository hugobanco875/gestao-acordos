namespace GestaoAcordos.Models;

public class ParcelaComprovante
{
    public int ParcelaId { get; set; }
    public Parcela? Parcela { get; set; }
    public byte[] Conteudo { get; set; } = Array.Empty<byte>();
}
