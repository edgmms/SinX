using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SinX.Web.ViewModels.Setup;
using SinX.Application.Abstractions.Setup;
using SinX.Application.Features.Setup.CompleteSetup;
using Microsoft.Extensions.Localization;
using SinX.Web;

namespace SinX.Web.Controllers;

[AllowAnonymous]
[Route("setup")]
public sealed class SetupController : Controller
{
    private readonly ISetupStateStore _stateStore;
    private readonly ITransientSetupSecretStore _secretStore;
    private readonly CompleteSetupHandler _handler;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public SetupController(ISetupStateStore stateStore, ITransientSetupSecretStore secretStore, CompleteSetupHandler handler, IStringLocalizer<SharedResource> localizer)
    { _stateStore = stateStore; _secretStore = secretStore; _handler = handler; _localizer = localizer; }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        if ((await _stateStore.GetAsync(cancellationToken)).IsCompleted)
        {
            return RedirectToAction("Index", "Login");
        }
        return View(new SetupInputModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SetupInputModel model, CancellationToken cancellationToken)
    {
        if ((await _stateStore.GetAsync(cancellationToken)).IsCompleted)
        {
            return RedirectToAction("Index", "Login");
        }
        if (!ModelState.IsValid || !string.Equals(model.Password, model.ConfirmPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(string.Empty, _localizer["SetupGenericError"]);
            return View(new SetupInputModel { UserName = model.UserName });
        }
        var secretReference = await _secretStore.StoreAsync(model.Password!, cancellationToken);
        var result = await _handler.HandleAsync(new CompleteSetupCommand(model.UserName!, secretReference, HttpContext.TraceIdentifier), cancellationToken);
        if (result.Succeeded || result.AlreadyCompleted)
        {
            return RedirectToAction("Index", "Login");
        }
        ModelState.AddModelError(string.Empty, _localizer["SetupGenericError"]);
        return View(new SetupInputModel { UserName = model.UserName });
    }
}
