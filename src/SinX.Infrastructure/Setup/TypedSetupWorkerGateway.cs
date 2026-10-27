using SinX.Application.Abstractions.Setup;
using SinX.Contracts.Worker;

namespace SinX.Infrastructure.Setup;

public sealed class TypedSetupWorkerGateway : ISetupWorkerGateway
{
    private readonly IWorkerDispatcherClient _dispatcher;

    public TypedSetupWorkerGateway(IWorkerDispatcherClient dispatcher) => _dispatcher = dispatcher;

    public async Task<SetupWorkerOutcome> ProvisionAsync(Guid operationId, string userName, string secretReference, string correlationId, CancellationToken cancellationToken)
    {
        var request = new WorkerDispatchRequest(1, "setup.provision", operationId, "bootstrap", "server", "local-administrator", secretReference, correlationId, userName);
        if (request.OperationId == Guid.Empty || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.SecretReference) || string.IsNullOrWhiteSpace(request.CorrelationId))
        {
            return new SetupWorkerOutcome(false, null, "setup.contract.invalid");
        }

        var response = await _dispatcher.DispatchAsync(request, cancellationToken);
        return new SetupWorkerOutcome(response.Succeeded, response.SubjectSid, response.ErrorCode);
    }
}
