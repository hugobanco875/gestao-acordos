using System.Text.Json.Serialization;

namespace GestaoAcordos.Services;

public record EnderecoCep(string Logradouro, string Bairro, string Cidade, string Uf);

/// <summary>Busca de endereço por CEP usando a API pública ViaCEP (grátis, sem chave).</summary>
public class EnderecoService(HttpClient http, ILogger<EnderecoService> logger)
{
    public async Task<EnderecoCep?> BuscarCepAsync(string? cep)
    {
        var digitos = new string((cep ?? "").Where(char.IsDigit).ToArray());
        if (digitos.Length != 8) return null;

        try
        {
            var r = await http.GetFromJsonAsync<ViaCepResposta>($"https://viacep.com.br/ws/{digitos}/json/");
            if (r is null || r.Erro) return null;
            return new EnderecoCep(r.Logradouro ?? "", r.Bairro ?? "", r.Localidade ?? "", r.Uf ?? "");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Falha ao buscar CEP {Cep}", digitos);
            return null;
        }
    }

    private class ViaCepResposta
    {
        [JsonPropertyName("logradouro")] public string? Logradouro { get; set; }
        [JsonPropertyName("bairro")] public string? Bairro { get; set; }
        [JsonPropertyName("localidade")] public string? Localidade { get; set; }
        [JsonPropertyName("uf")] public string? Uf { get; set; }
        [JsonPropertyName("erro")] public bool Erro { get; set; }
    }
}
