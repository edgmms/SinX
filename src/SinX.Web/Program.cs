using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SinX.Application.Abstractions.Setup;
using SinX.Application.Features.Setup.CompleteSetup;
using SinX.Application.Abstractions.Authentication;
using SinX.Application.Features.Authentication.Login;
using SinX.Infrastructure.Authentication;
using SinX.Infrastructure.Setup;
using SinX.Web.Transport;
using SinX.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWindowsService();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(8080);
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddDataProtection();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddOptions<LoginSecurityOptions>()
    .Bind(builder.Configuration.GetSection(LoginSecurityOptions.SectionName))
    .Validate(options => options.MaximumFailures == 5 && options.FailureWindow == TimeSpan.FromMinutes(15) && options.LockoutDuration == TimeSpan.FromMinutes(15) && options.IdleTimeout == TimeSpan.FromMinutes(20), "Login security defaults must remain 5/15/15/20.")
    .ValidateOnStart();
builder.Services.AddDbContext<SetupDbContext>(options => options.UseSqlite("Data Source=sinx.db"));
builder.Services.AddScoped<EfSetupStores>();
builder.Services.AddScoped<ISetupStateStore>(provider => provider.GetRequiredService<EfSetupStores>());
builder.Services.AddScoped<ITransientSetupSecretStore>(provider => provider.GetRequiredService<EfSetupStores>());
builder.Services.AddScoped<ISetupAuditSink>(provider => provider.GetRequiredService<EfSetupStores>());
builder.Services.AddSingleton<SinX.Contracts.Worker.IWorkerDispatcherClient, NamedPipeWorkerDispatcherClient>();
builder.Services.AddScoped<ISetupWorkerGateway, TypedSetupWorkerGateway>();
builder.Services.AddScoped<CompleteSetupHandler>();
builder.Services.AddScoped<EfLoginSecurityStore>();
builder.Services.AddScoped<ILoginAttemptStore>(provider => provider.GetRequiredService<EfLoginSecurityStore>());
builder.Services.AddScoped<ILoginAuditSink>(provider => provider.GetRequiredService<EfLoginSecurityStore>());
builder.Services.AddScoped<ICredentialValidationGateway, TypedCredentialValidationGateway>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options =>
        options.DataAnnotationLocalizerProvider = (_, factory) => factory.Create(typeof(SharedResource)));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var setupDb = scope.ServiceProvider.GetRequiredService<SetupDbContext>();
    setupDb.Database.Migrate();
}

app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("tr-TR"),
    SupportedCultures = new[] { new CultureInfo("tr-TR"), new CultureInfo("en-US") },
    SupportedUICultures = new[] { new CultureInfo("tr-TR"), new CultureInfo("en-US") },
    RequestCultureProviders = new[]
    {
        new CustomRequestCultureProvider(context =>
        {
            var cookie = context.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var parsedCulture = cookie is null ? null : CookieRequestCultureProvider.ParseCookieValue(cookie);
            var culture = parsedCulture?.Cultures.FirstOrDefault().Value;
            if (culture is "tr-TR" or "en-US")
            {
                return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture, culture));
            }

            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult("tr-TR", "tr-TR"));
        })
    }
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallback(() => Results.NotFound()).RequireAuthorization();

app.Run();

public partial class Program;
