using SinX.Contracts.Worker;
using SinX.Worker.Operations.Setup;

namespace SinX.Worker.Operations;

/// <summary>Worker-side allowlist. A named-pipe host supplies authenticated local requests to this dispatcher.</summary>
public sealed class WorkerDispatcher
{
    private const int ContractVersion = 1;
    private readonly SetupOperationHandler _setup;
    private readonly ILoginOperationHandler _login;

    public WorkerDispatcher(SetupOperationHandler setup, ILoginOperationHandler login)
    {
        _setup = setup;
        _login = login;
    }

    public async Task<WorkerDispatchResponse> DispatchAsync(WorkerDispatchRequest request, CancellationToken cancellationToken)
    {
        if (request.ContractVersion != ContractVersion || request.OperationId == Guid.Empty ||
            string.IsNullOrWhiteSpace(request.Actor) || string.IsNullOrWhiteSpace(request.Scope) ||
            string.IsNullOrWhiteSpace(request.Target) || string.IsNullOrWhiteSpace(request.CorrelationId) ||
            string.IsNullOrWhiteSpace(request.SecretReference))
        {
            return new WorkerDispatchResponse(false, null, "worker.contract.invalid");
        }

        return request.Operation switch
        {
            "setup.provision" when request.Actor == "bootstrap" && request.Scope == "server" && request.Target == "local-administrator" && !string.IsNullOrWhiteSpace(request.UserName)
                => await _setup.HandleAsync(request, cancellationToken),
            "login.validate" when request.Actor == "anonymous" && request.Scope == "server" && request.Target == "bootstrap-administrator" && !string.IsNullOrWhiteSpace(request.UserName)
                => await _login.HandleAsync(request, cancellationToken),
            _ => new WorkerDispatchResponse(false, null, "worker.operation.rejected")
        };
    }
}

public interface ILoginOperationHandler
{
    Task<WorkerDispatchResponse> HandleAsync(WorkerDispatchRequest request, CancellationToken cancellationToken);
}
