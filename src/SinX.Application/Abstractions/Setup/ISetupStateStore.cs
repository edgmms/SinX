namespace SinX.Application.Abstractions.Setup;

public interface ISetupStateStore
{
    Task<SetupStateSnapshot> GetAsync(CancellationToken cancellationToken);
    Task<bool> TryBeginAsync(Guid operationId, CancellationToken cancellationToken);
    Task CompleteAsync(Guid operationId, string acceptedSid, CancellationToken cancellationToken);
    Task FailAsync(Guid operationId, string errorCode, CancellationToken cancellationToken);
}

public sealed record SetupStateSnapshot(bool IsCompleted, bool IsInProgress, string? AcceptedSid);
