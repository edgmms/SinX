using SinX.Contracts.Worker;

namespace SinX.Infrastructure.Setup;

/// <summary>
/// Safe composition placeholder until the Windows service installs the ACL-protected named-pipe client.
/// It deliberately cannot execute any privileged operation in the Web process.
/// </summary>
public sealed class UnavailableWorkerDispatcherClient : IWorkerDispatcherClient
{
    public Task<WorkerDispatchResponse> DispatchAsync(WorkerDispatchRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new WorkerDispatchResponse(false, null, "worker.unavailable"));
}
