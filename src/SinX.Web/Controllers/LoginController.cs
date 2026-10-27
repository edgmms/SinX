using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SinX.Web.ViewModels.Account;
using SinX.Application.Abstractions.Setup;
using SinX.Application.Features.Authentication.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using SinX.Web;

namespace SinX.Web.Controllers;

[Route("login")]
public sealed class LoginController : Controller
{
    private readonly ISetupStateStore _setupState;
    private readonly ITransientSetupSecretStore _secretStore;
    private readonly LoginHandler _handler;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public LoginController(ISetupStateStore setupState, ITransientSetupSecretStore secretStore, LoginHandler handler, IStringLocalizer<SharedResource> localizer)
    {
        _setupState = setupState;
        _secretStore = secretStore;
        _handler = handler;
        _localizer = localizer;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index([FromQuery] string? returnUrl, CancellationToken cancellationToken)
    {
        if (!(await _setupState.GetAsync(cancellationToken)).IsCompleted)
        {
            return RedirectToAction("Index", "Setup");
        }
        var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null;

        return View(new LoginInputModel { ReturnUrl = safeReturnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginInputModel model, CancellationToken cancellationToken)
    {
        if (!(await _setupState.GetAsync(cancellationToken)).IsCompleted)
        {
            return RedirectToAction("Index", "Setup");
        }

        var returnUrl = Url.IsLocalUrl(model.ReturnUrl) ? model.ReturnUrl : null;
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, _localizer["LoginGenericError"]);
            return View(new LoginInputModel { UserName = model.UserName, ReturnUrl = returnUrl });
        }

        var secretReference = await _secretStore.StoreAsync(model.Password!, cancellationToken);
        var result = await _handler.HandleAsync(new LoginCommand(model.UserName!, secretReference, HttpContext.TraceIdentifier), cancellationToken);
        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.Sid))
        {
            ModelState.AddModelError(string.Empty, _localizer["LoginGenericError"]);
            return View(new LoginInputModel { UserName = model.UserName, ReturnUrl = returnUrl });
        }

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, result.Sid),
            new Claim(ClaimTypes.Name, model.UserName!)
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), new AuthenticationProperties { IsPersistent = false, AllowRefresh = true });
        return returnUrl is not null ? LocalRedirect(returnUrl) : Redirect("/");
    }

    [HttpPost("/logout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }
}
