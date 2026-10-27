using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SinX.Web.Tests;

public sealed class WebRouteTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public WebRouteTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [InlineData("/setup")]
    public async Task PublicPage_WhenRequested_ShouldReturnSuccessAsync(string path)
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync(path);

        Assert.True(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Login_WhenSetupIsIncomplete_ShouldRedirectToSetupAsync()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/login");

        Assert.Equal(StatusCodes.Status302Found, (int)response.StatusCode);
        Assert.Equal("/setup", response.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData("/setup", "Kurulumu tamamla")]
    public async Task PublicMvcView_WhenRendered_ShouldUseLocalAssetsAndAntiForgery(string path, string submitText)
    {
        using var client = _factory.CreateClient();

        var markup = await client.GetStringAsync(path);

        Assert.Contains(submitText, markup, StringComparison.Ordinal);
        Assert.Contains("__RequestVerificationToken", markup, StringComparison.Ordinal);
        Assert.Contains("/vendor/gentelella/main-v4-DB_ReeJG.css", markup, StringComparison.Ordinal);
        Assert.DoesNotContain("https://", markup, StringComparison.Ordinal);
        Assert.DoesNotContain("http://", markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GentelellaAsset_WhenRequested_ShouldBeLocalWithoutExternalImports()
    {
        using var client = _factory.CreateClient();

        var css = await client.GetStringAsync("/vendor/gentelella/main-v4-DB_ReeJG.css");

        Assert.NotEmpty(css);
        Assert.DoesNotContain("url(http", css, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("@import", css, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("c=en-US|uic=en-US", "Create the administrator account")]
    [InlineData("c=invalid|uic=invalid", "Yönetici hesabını oluşturun")]
    public async Task CultureCookie_WhenAllowedOrMalformed_ShouldRenderAllowedCultureOrTurkishFallback(string cultureValue, string expectedHeading)
    {
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/setup");
        request.Headers.Add("Cookie", ".AspNetCore.Culture=" + cultureValue);

        using var response = await client.SendAsync(request);
        var markup = await response.Content.ReadAsStringAsync();

        Assert.Contains(expectedHeading, markup, StringComparison.Ordinal);
    }

    [Fact]
    public async Task UnknownRoute_WhenRequestedAnonymously_ShouldRedirectToLoginAsync()
    {
        using var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        using var response = await client.GetAsync("/not-public");

        Assert.Equal(StatusCodes.Status302Found, (int)response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.StartsWith("/login?ReturnUrl=", response.Headers.Location.PathAndQuery, StringComparison.Ordinal);
    }

    [Fact]
    public void DevelopmentHostConfiguration_ShouldUseOnlyLocalhostPort8080()
    {
        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        var programSourcePath = Path.Combine(environment.ContentRootPath, "Program.cs");
        var programSource = File.ReadAllText(programSourcePath);

        Assert.Contains("ListenLocalhost(8080)", programSource, StringComparison.Ordinal);
        Assert.DoesNotContain("57500", programSource, StringComparison.Ordinal);
        Assert.DoesNotContain("ListenAnyIP", programSource, StringComparison.Ordinal);
    }

    [Fact]
    public void MvcSurface_ShouldNotContainRazorPagesOrBlazorArtifacts()
    {
        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        var programSourcePath = Path.Combine(environment.ContentRootPath, "Program.cs");

        Assert.False(Directory.Exists(Path.Combine(environment.ContentRootPath, "Pages")));
        Assert.Empty(Directory.EnumerateFiles(environment.ContentRootPath, "*.razor", SearchOption.AllDirectories));
        Assert.DoesNotContain("AddRazorPages", File.ReadAllText(programSourcePath), StringComparison.Ordinal);
        Assert.DoesNotContain("MapRazorPages", File.ReadAllText(programSourcePath), StringComparison.Ordinal);
        Assert.DoesNotContain(
            typeof(Program).Assembly.GetReferencedAssemblies(),
            assembly => assembly.Name?.StartsWith("Microsoft.AspNetCore.Components", StringComparison.Ordinal) == true);
    }

    [Fact]
    public void SecuritySensitiveMvcEndpoints_ShouldRequireAntiForgeryAndAvoidPasswordEcho()
    {
        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        var setupSource = File.ReadAllText(Path.Combine(environment.ContentRootPath, "Controllers", "SetupController.cs"));
        var loginSource = File.ReadAllText(Path.Combine(environment.ContentRootPath, "Controllers", "LoginController.cs"));

        Assert.Contains("[ValidateAntiForgeryToken]", setupSource, StringComparison.Ordinal);
        Assert.Contains("[ValidateAntiForgeryToken]", loginSource, StringComparison.Ordinal);
        Assert.Contains("Url.IsLocalUrl", loginSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Password = model.Password", setupSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Password = model.Password", loginSource, StringComparison.Ordinal);
    }

    [Fact]
    public void CookieAndWorkerBoundary_ShouldUseSafeDefaults()
    {
        var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
        var programSource = File.ReadAllText(Path.Combine(environment.ContentRootPath, "Program.cs"));
        var sourceRoot = Path.GetFullPath(Path.Combine(environment.ContentRootPath, ".."));
        var contractSource = File.ReadAllText(Path.Combine(sourceRoot, "SinX.Contracts", "Worker", "WorkerDispatchContract.cs"));

        Assert.Contains("SameSiteMode.Strict", programSource, StringComparison.Ordinal);
        Assert.Contains("CookieSecurePolicy.SameAsRequest", programSource, StringComparison.Ordinal);
        Assert.Contains("TimeSpan.FromMinutes(20)", programSource, StringComparison.Ordinal);
        Assert.Contains("SecretReference", contractSource, StringComparison.Ordinal);
        Assert.DoesNotContain("Password", contractSource, StringComparison.Ordinal);
    }
}
