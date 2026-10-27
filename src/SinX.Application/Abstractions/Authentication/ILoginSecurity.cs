namespace SinX.Application.Abstractions.Authentication;

public interface ICredentialValidationGateway
{
    Task<CredentialValidationOutcome> ValidateAsync(string userName, string secretReference, string correlationId, CancellationToken cancellationToken);
}

public sealed record CredentialValidationOutcome(bool Succeeded, string? Sid, string ErrorCode);

public interface ILoginAttemptStore
{
    Task<LoginAttemptStatus> GetStatusAsync(string userName, CancellationToken cancellationToken);
    Task RecordFailureAsync(string userName, CancellationToken cancellationToken);
    Task RecordSuccessAsync(string userName, CancellationToken cancellationToken);
}

public sealed record LoginAttemptStatus(bool IsLockedOut);

public interface ILoginAuditSink
{
    Task WriteAsync(string outcome, string correlationId, CancellationToken cancellationToken);
}
