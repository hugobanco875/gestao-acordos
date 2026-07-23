namespace GestaoAcordos.Services;

/// <summary>
/// Fonte única de data e hora do sistema no fuso de Sergipe/Brasília (UTC-3).
/// Evita que o horário dependa do fuso configurado no servidor ou contêiner.
/// </summary>
public static class RelogioSistema
{
    private static readonly TimeZoneInfo FusoHorario = ObterFusoHorario();

    public static DateTime Agora =>
        DateTime.SpecifyKind(
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, FusoHorario),
            DateTimeKind.Unspecified);

    public static DateOnly Hoje => DateOnly.FromDateTime(Agora);

    public static string IdFusoHorario => FusoHorario.Id;

    private static TimeZoneInfo ObterFusoHorario()
    {
        // Linux, Docker e serviços como Render/Fly.io.
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Maceio");
        }
        catch (Exception ex) when (ex is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            // Windows sem suporte aos identificadores IANA.
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            }
            catch (Exception exFallback) when (exFallback is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                // Último recurso: UTC-3 fixo. Sergipe não utiliza horário de verão.
                return TimeZoneInfo.CreateCustomTimeZone(
                    "America/Maceio-Fallback",
                    TimeSpan.FromHours(-3),
                    "Horário de Sergipe",
                    "Horário de Sergipe");
            }
        }
    }
}
