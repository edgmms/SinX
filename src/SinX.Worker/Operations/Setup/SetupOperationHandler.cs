using SinX.Contracts.Worker;
using SinX.Windows.Setup;

namespace SinX.Worker.Operations.Setup;

public interface IWorkerSecretResolver
{
    Task<string?> ConsumeAsync(string secretReference, CancellationToken cancellationToken);
}

public sealed class SetupOperationHandler
{
    private readonly IWorkerSecretResolver _secretResolver;
    private readonly IWindowsSetupPlatform _platform;

    public SetupOperationHandler(IWorkerSecretResolver secretResolver, IWindowsSetupPlatform platform)
    {
        _secretResolver = secretResolver;
        _platform = platform;
    }

    public async Task<WorkerDispatchResponse> HandleAsync(WorkerDispatchRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.SecretReference))
        {
            return new WorkerDispatchResponse(false, null, "setup.contract.invalid");
        }
        var password = await _secretResolver.ConsumeAsync(request.SecretReference, cancellationToken);
        if (password is null)
        {
            return new WorkerDispatchResponse(false, null, "setup.secret.unavailable");
        }
        try
        {
            var result = await _platform.ProvisionAsync(request.UserName, password, cancellationToken);
            return new WorkerDispatchResponse(result.Succeeded, result.Sid, result.ErrorCode);
        }
        finally
        {
            password = string.Empty;
        }
    }
}
