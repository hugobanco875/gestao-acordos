namespace GestaoAcordos.Models;

/// <summary>Vínculo entre um cliente e as empresas às quais ele pertence.</summary>
public class ClienteEmpresa
{
    public int ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }
}
