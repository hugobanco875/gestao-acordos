using System.Text.Json.Serialization;

namespace GestaoAcordos.Services;

public record DadosCnpj(
    string RazaoSocial, string NomeFantasia, string Telefone, string Email,
    string Cep, string Logradouro, string Numero, string Bairro, string Cidade, string Uf);

/// <summary>Busca de dados de empresa por CNPJ usando a BrasilAPI (grátis, sem chave).</summary>
public class CnpjService(HttpClient http, ILogger<CnpjService> logger)
{
    public async Task<DadosCnpj?> BuscarAsync(string? cnpj)
    {
        var digitos = new string((cnpj ?? "").Where(char.IsDigit).ToArray());
        if (digitos.Length != 14) return null;

        try
        {
            var r = await http.GetFromJsonAsync<BrasilApiCnpj>($"https://brasilapi.com.br/api/cnpj/v1/{digitos}");
            if (r is null) return null;
            return new DadosCnpj(
                r.RazaoSocial ?? "",
                r.NomeFantasia ?? "",
                r.Telefone ?? "",
                r.Email ?? "",
                r.Cep ?? "",
                r.Logradouro ?? "",
                r.Numero ?? "",
                r.Bairro ?? "",
                r.Municipio ?? "",
                r.Uf ?? "");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao buscar CNPJ {Cnpj}", digitos);
            return null;
        }
    }

    private class BrasilApiCnpj
    {
        [JsonPropertyName("razao_social")] public string? RazaoSocial { get; set; }
        [JsonPropertyName("nome_fantasia")] public string? NomeFantasia { get; set; }
        [JsonPropertyName("ddd_telefone_1")] public string? Telefone { get; set; }
        [JsonPropertyName("email")] public string? Email { get; set; }
        [JsonPropertyName("cep")] public string? Cep { get; set; }
        [JsonPropertyName("logradouro")] public string? Logradouro { get; set; }
        [JsonPropertyName("numero")] public string? Numero { get; set; }
        [JsonPropertyName("bairro")] public string? Bairro { get; set; }
        [JsonPropertyName("municipio")] public string? Municipio { get; set; }
        [JsonPropertyName("uf")] public string? Uf { get; set; }
    }
}
