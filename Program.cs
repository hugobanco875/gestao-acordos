using System.Globalization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using GestaoAcordos.Components;
using GestaoAcordos.Components.Account;
using GestaoAcordos.Data;
using GestaoAcordos.Services;

var builder = WebApplication.CreateBuilder(args);

// Licenca gratuita do QuestPDF (geracao de PDF dos relatorios).
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

// Trabalhar com datas/hora no horario local (sem conversao UTC obrigatoria do Npgsql).
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Formatacao pt-BR (R$, datas dd/MM/yyyy).
var culturaPtBr = new CultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = culturaPtBr;
CultureInfo.DefaultThreadCurrentUICulture = culturaPtBr;

// Porta dinâmica em hospedagem gratuita (Render/Fly.io definem a variável PORT).
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://+:{port}");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Factory para uso seguro do DbContext nos componentes Blazor Server...
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);

    // Algumas migrations de compatibilidade foram escritas manualmente. O aviso
    // de snapshot não deve impedir a atualização automática; as migrations
    // continuam sendo identificadas por seus atributos e aplicadas em ordem.
    options.ConfigureWarnings(warnings =>
        warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});
// ...e um contexto "scoped" (criado pela factory) para o ASP.NET Identity.
builder.Services.AddScoped<ApplicationDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Persiste as chaves de proteção de dados no banco, para que cookies e tokens
// antifalsificação sobrevivam a reinícios/deploys (evita erro no logout).
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        // Regras de senha mais simples (app interno).
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddErrorDescriber<PtBrIdentityErrorDescriber>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<BackupService>();
builder.Services.AddScoped<ConfiguracaoService>();
builder.Services.AddHttpClient<EnderecoService>();
builder.Services.AddHttpClient<CnpjService>();

var app = builder.Build();

// Aplica migrations automaticamente na inicializacao (cria/atualiza as tabelas).
using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    using var db = dbFactory.CreateDbContext();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine("Falha ao atualizar a estrutura do banco de dados.");
        Console.Error.WriteLine(ex);
        throw;
    }
}

// Cultura pt-BR na primeira renderizacao (server-side).
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(culturaPtBr),
    SupportedCultures = new[] { culturaPtBr },
    SupportedUICultures = new[] { culturaPtBr }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
