using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestaoAcordos.Services;

public record DadosCnpj(
    string RazaoSocial, string NomeFantasia, string Telefone, string Email,
    string Cep, string Logradouro, string Numero, string Bairro, string Cidade, string Uf);

/// <summary>
/// Busca dados de empresa por CNPJ. Tenta a BrasilAPI e, se ela estiver
/// indisponível ou limitar a consulta, utiliza a ReceitaWS como contingência.
/// </summary>
public class CnpjService
{
    private readonly HttpClient _http;
    private readonly ILogger<CnpjService> _logger;

    public string? UltimaMensagem { get; private set; }

    public CnpjService(HttpClient http, ILogger<CnpjService> logger)
    {
        _http = http;
        _logger = logger;
        _http.Timeout = TimeSpan.FromSeconds(15);

        if (!_http.DefaultRequestHeaders.UserAgent.Any())
            _http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GestaoAcordos", "1.0"));

        _http.DefaultRequestHeaders.Accept.Clear();
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<DadosCnpj?> BuscarAsync(string? cnpj)
    {
        UltimaMensagem = null;
        var digitos = SomenteDigitos(cnpj);

        if (digitos.Length != 14)
        {
            UltimaMensagem = "Informe os 14 números do CNPJ.";
            return null;
        }

        if (!CnpjValido(digitos))
        {
            UltimaMensagem = "O CNPJ informado é inválido. Confira os números.";
            return null;
        }

        // Provedor principal
        var brasilApi = await BuscarNaBrasilApiAsync(digitos);
        if (brasilApi is not null)
            return brasilApi;

        // Contingência para indisponibilidade, bloqueio ou limite do provedor principal
        var receitaWs = await BuscarNaReceitaWsAsync(digitos);
        if (receitaWs is not null)
            return receitaWs;

        UltimaMensagem ??= "Não foi possível consultar o CNPJ agora. Verifique sua internet e tente novamente em alguns instantes.";
        return null;
    }

    private async Task<DadosCnpj?> BuscarNaBrasilApiAsync(string cnpj)
    {
        try
        {
            using var response = await _http.GetAsync($"https://brasilapi.com.br/api/cnpj/v1/{cnpj}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                UltimaMensagem = "CNPJ não encontrado na base pública.";
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("BrasilAPI retornou HTTP {StatusCode} para o CNPJ {Cnpj}", (int)response.StatusCode, cnpj);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            var r = await JsonSerializer.DeserializeAsync<BrasilApiCnpj>(stream, JsonOptions);
            if (r is null || string.IsNullOrWhiteSpace(r.RazaoSocial))
                return null;

            UltimaMensagem = null;
            return new DadosCnpj(
                r.RazaoSocial ?? "",
                r.NomeFantasia ?? "",
                r.Telefone ?? "",
                r.Email ?? "",
                FormatarCep(r.Cep),
                r.Logradouro ?? "",
                r.Numero ?? "",
                r.Bairro ?? "",
                r.Municipio ?? "",
                r.Uf ?? "");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Tempo excedido ao consultar a BrasilAPI para o CNPJ {Cnpj}", cnpj);
            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "Falha de conexão com a BrasilAPI para o CNPJ {Cnpj}", cnpj);
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Resposta inválida da BrasilAPI para o CNPJ {Cnpj}", cnpj);
            return null;
        }
    }

    private async Task<DadosCnpj?> BuscarNaReceitaWsAsync(string cnpj)
    {
        try
        {
            using var response = await _http.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpj}");

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                UltimaMensagem = "O serviço público de consulta atingiu o limite temporário. Aguarde um minuto e tente novamente.";
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ReceitaWS retornou HTTP {StatusCode} para o CNPJ {Cnpj}", (int)response.StatusCode, cnpj);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var json = await JsonDocument.ParseAsync(stream);
            var root = json.RootElement;

            var status = GetString(root, "status");
            if (status.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
            {
                var mensagem = GetString(root, "message");
                UltimaMensagem = string.IsNullOrWhiteSpace(mensagem)
                    ? "CNPJ não encontrado na base pública."
                    : mensagem;
                return null;
            }

            var nome = GetString(root, "nome");
            if (string.IsNullOrWhiteSpace(nome))
                return null;

            UltimaMensagem = null;
            return new DadosCnpj(
                nome,
                GetString(root, "fantasia"),
                GetString(root, "telefone"),
                GetString(root, "email"),
                FormatarCep(GetString(root, "cep")),
                GetString(root, "logradouro"),
                GetString(root, "numero"),
                GetString(root, "bairro"),
                GetString(root, "municipio"),
                GetString(root, "uf"));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Tempo excedido ao consultar a ReceitaWS para o CNPJ {Cnpj}", cnpj);
            return null;
        }
        catch (Exception ex) when (ex is HttpRequestException or JsonException)
        {
            _logger.LogWarning(ex, "Falha ao consultar a ReceitaWS para o CNPJ {Cnpj}", cnpj);
            return null;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static string GetString(JsonElement root, string property)
    {
        if (!root.TryGetProperty(property, out var value) || value.ValueKind == JsonValueKind.Null)
            return "";

        return value.ValueKind == JsonValueKind.String ? value.GetString() ?? "" : value.ToString();
    }

    private static string SomenteDigitos(string? valor) =>
        new((valor ?? "").Where(char.IsDigit).ToArray());

    private static string FormatarCep(string? cep)
    {
        var d = SomenteDigitos(cep);
        return d.Length == 8 ? $"{d[..5]}-{d[5..]}" : cep?.Trim() ?? "";
    }

    private static bool CnpjValido(string cnpj)
    {
        if (cnpj.Length != 14 || cnpj.All(c => c == cnpj[0]))
            return false;

        static int CalcularDigito(string baseCnpj, int[] pesos)
        {
            var soma = 0;
            for (var i = 0; i < pesos.Length; i++)
                soma += (baseCnpj[i] - '0') * pesos[i];

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        var primeiro = CalcularDigito(cnpj[..12], [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);
        var segundo = CalcularDigito(cnpj[..13], [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);

        return primeiro == cnpj[12] - '0' && segundo == cnpj[13] - '0';
    }

    private sealed class BrasilApiCnpj
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
