namespace SinX.Application.Abstractions.Setup;

public interface ISetupAuditSink
{
    Task WriteAsync(string eventType, string outcome, string correlationId, CancellationToken cancellationToken);
}
