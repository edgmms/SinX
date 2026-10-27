using SinX.Application.Abstractions.Setup;

namespace SinX.Application.Features.Setup.CompleteSetup;

public sealed class CompleteSetupHandler
{
    private readonly ISetupStateStore _stateStore;
    private readonly ISetupWorkerGateway _workerGateway;
    private readonly ISetupAuditSink _auditSink;

    public CompleteSetupHandler(ISetupStateStore stateStore, ISetupWorkerGateway workerGateway, ISetupAuditSink auditSink)
    {
        _stateStore = stateStore;
        _workerGateway = workerGateway;
        _auditSink = auditSink;
    }

    public async Task<CompleteSetupResult> HandleAsync(CompleteSetupCommand command, CancellationToken cancellationToken)
    {
        if (!IsValidUserName(command.UserName))
        {
            return CompleteSetupResult.Failure("setup.username.invalid");
        }
        if (string.IsNullOrWhiteSpace(command.SecretReference))
        {
            return CompleteSetupResult.Failure("setup.secret.invalid");
        }
        var state = await _stateStore.GetAsync(cancellationToken);
        if (state.IsCompleted)
        {
            return CompleteSetupResult.Completed();
        }
        var operationId = Guid.NewGuid();
        if (!await _stateStore.TryBeginAsync(operationId, cancellationToken))
        {
            return CompleteSetupResult.Failure("setup.in_progress");
        }
        var outcome = await _workerGateway.ProvisionAsync(operationId, command.UserName, command.SecretReference, command.CorrelationId, cancellationToken);
        if (!outcome.Succeeded || string.IsNullOrWhiteSpace(outcome.AcceptedSid))
        {
            await _stateStore.FailAsync(operationId, outcome.ErrorCode, cancellationToken);
            await _auditSink.WriteAsync("setup.completed", "failed", command.CorrelationId, cancellationToken);
            return CompleteSetupResult.Failure(outcome.ErrorCode);
        }
        await _stateStore.CompleteAsync(operationId, outcome.AcceptedSid, cancellationToken);
        await _auditSink.WriteAsync("setup.completed", "succeeded", command.CorrelationId, cancellationToken);
        return CompleteSetupResult.Success();
    }

    private static bool IsValidUserName(string userName) => !string.IsNullOrWhiteSpace(userName) && userName.Length is <= 20 && userName.All(char.IsLetterOrDigit);
}
