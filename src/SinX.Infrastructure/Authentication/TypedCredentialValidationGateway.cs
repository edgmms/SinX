using SinX.Application.Abstractions.Authentication;
using SinX.Contracts.Worker;

namespace SinX.Infrastructure.Authentication;

public sealed class TypedCredentialValidationGateway : ICredentialValidationGateway
{
    private readonly IWorkerDispatcherClient _dispatcher;

    public TypedCredentialValidationGateway(IWorkerDispatcherClient dispatcher) => _dispatcher = dispatcher;

    public async Task<CredentialValidationOutcome> ValidateAsync(string userName, string secretReference, string correlationId, CancellationToken cancellationToken)
    {
        var request = new WorkerDispatchRequest(1, "login.validate", Guid.NewGuid(), "anonymous", "server", "bootstrap-administrator", secretReference, correlationId, userName);
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(secretReference))
        {
            return new CredentialValidationOutcome(false, null, "login.contract.invalid");
        }

        var response = await _dispatcher.DispatchAsync(request, cancellationToken);
        return new CredentialValidationOutcome(response.Succeeded, response.SubjectSid, response.ErrorCode);
    }
}
