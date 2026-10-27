namespace SinX.Application.Abstractions.Setup;

public interface ISetupWorkerGateway
{
    Task<SetupWorkerOutcome> ProvisionAsync(
        Guid operationId,
        string userName,
        string secretReference,
        string correlationId,
        CancellationToken cancellationToken);
}

public sealed record SetupWorkerOutcome(bool Succeeded, string? AcceptedSid, string ErrorCode);
