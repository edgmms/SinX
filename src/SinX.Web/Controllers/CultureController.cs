using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace SinX.Web.Controllers;

[AllowAnonymous]
[Route("culture")]
public sealed class CultureController : Controller
{
    private static readonly HashSet<string> AllowedCultures = new(StringComparer.OrdinalIgnoreCase) { "tr-TR", "en-US" };

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Set(string culture, string? returnUrl)
    {
        var selectedCulture = AllowedCultures.Contains(culture) ? culture : "tr-TR";
        var cultureInfo = CultureInfo.GetCultureInfo(selectedCulture);
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureInfo)),
            new CookieOptions { HttpOnly = true, IsEssential = true, SameSite = SameSiteMode.Lax });

        return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl! : "/setup");
    }
}
