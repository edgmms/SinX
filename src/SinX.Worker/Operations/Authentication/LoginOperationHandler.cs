using SinX.Contracts.Worker;
using SinX.Windows.Authentication;
using SinX.Worker.Operations.Setup;

namespace SinX.Worker.Operations.Authentication;

public sealed class LoginOperationHandler : ILoginOperationHandler
{
    private readonly IWorkerSecretResolver _secretResolver;
    private readonly IWindowsCredentialValidator _validator;

    public LoginOperationHandler(IWorkerSecretResolver secretResolver, IWindowsCredentialValidator validator)
    {
        _secretResolver = secretResolver;
        _validator = validator;
    }

    public async Task<WorkerDispatchResponse> HandleAsync(WorkerDispatchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            return new WorkerDispatchResponse(false, null, "login.contract.invalid");
        }
        var password = await _secretResolver.ConsumeAsync(request.SecretReference, cancellationToken);
        if (password is null)
        {
            return new WorkerDispatchResponse(false, null, "login.secret.unavailable");
        }
        try
        {
            var result = await _validator.ValidateAsync(request.UserName, password, cancellationToken);
            return new WorkerDispatchResponse(result.Succeeded, result.Sid, result.ErrorCode);
        }
        finally
        {
            password = string.Empty;
        }
    }
}
