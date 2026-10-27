using Grpc.Core;
using SinX.Contracts.Worker;
using SinX.Contracts.Worker.Grpc;
using SinX.Worker.Operations;

namespace SinX.Worker.Transport;

public sealed class WorkerDispatchGrpcService : WorkerDispatch.WorkerDispatchBase
{
    private readonly WorkerDispatcher _dispatcher;

    public WorkerDispatchGrpcService(WorkerDispatcher dispatcher) => _dispatcher = dispatcher;

    public override async Task<DispatchResponse> Dispatch(DispatchRequest request, ServerCallContext context)
    {
        var operationId = Guid.TryParse(request.OperationId, out var parsed) ? parsed : Guid.Empty;
        var response = await _dispatcher.DispatchAsync(new WorkerDispatchRequest(
            request.ContractVersion, request.Operation, operationId, request.Actor, request.Scope,
            request.Target, request.SecretReference, request.CorrelationId,
            string.IsNullOrWhiteSpace(request.UserName) ? null : request.UserName), context.CancellationToken);
        return new DispatchResponse { Succeeded = response.Succeeded, SubjectSid = response.SubjectSid ?? string.Empty, ErrorCode = response.ErrorCode };
    }
}
