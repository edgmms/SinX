using System.IO.Pipes;
using System.Net.Sockets;
using Grpc.Net.Client;
using SinX.Contracts.Worker;
using SinX.Contracts.Worker.Grpc;

namespace SinX.Web.Transport;

/// <summary>Typed, local-only gRPC client. The Worker pipe ACL is the OS authorization boundary.</summary>
public sealed class NamedPipeWorkerDispatcherClient : IWorkerDispatcherClient, IDisposable
{
    private const string PipeName = "sinx-worker-v1";
    private readonly GrpcChannel _channel;
    private readonly WorkerDispatch.WorkerDispatchClient _client;

    public NamedPipeWorkerDispatcherClient()
    {
        var handler = new SocketsHttpHandler
        {
            ConnectCallback = async (_, cancellationToken) =>
            {
                var stream = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
                await stream.ConnectAsync(cancellationToken);
                return stream;
            }
        };
        _channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions { HttpHandler = handler });
        _client = new WorkerDispatch.WorkerDispatchClient(_channel);
    }

    public async Task<WorkerDispatchResponse> DispatchAsync(WorkerDispatchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _client.DispatchAsync(new DispatchRequest
            {
                ContractVersion = request.ContractVersion,
                Operation = request.Operation,
                OperationId = request.OperationId.ToString("D"),
                Actor = request.Actor,
                Scope = request.Scope,
                Target = request.Target,
                SecretReference = request.SecretReference,
                CorrelationId = request.CorrelationId,
                UserName = request.UserName ?? string.Empty
            }, cancellationToken: cancellationToken);
            return new WorkerDispatchResponse(response.Succeeded,
                string.IsNullOrWhiteSpace(response.SubjectSid) ? null : response.SubjectSid,
                response.ErrorCode);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception) when (IsTransportFailure())
        {
            return new WorkerDispatchResponse(false, null, "setup.worker.unavailable");
        }
    }

    private static bool IsTransportFailure() => true;

    public void Dispose() => _channel.Dispose();
}
