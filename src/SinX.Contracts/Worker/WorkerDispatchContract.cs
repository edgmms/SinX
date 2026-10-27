namespace SinX.Contracts.Worker;

/// <summary>Local-only IPC boundary. Implementations must authenticate the peer with the pipe ACL.</summary>
public interface IWorkerDispatcherClient
{
    Task<WorkerDispatchResponse> DispatchAsync(WorkerDispatchRequest request, CancellationToken cancellationToken);
}

public sealed record WorkerDispatchRequest(
    int ContractVersion,
    string Operation,
    Guid OperationId,
    string Actor,
    string Scope,
    string Target,
    string SecretReference,
    string CorrelationId,
    string? UserName);

public sealed record WorkerDispatchResponse(bool Succeeded, string? SubjectSid, string ErrorCode);
