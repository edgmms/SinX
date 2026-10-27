using SinX.Application.Abstractions.Authentication;
using SinX.Application.Abstractions.Setup;

namespace SinX.Application.Features.Authentication.Login;

public sealed class LoginHandler
{
    private readonly ISetupStateStore _setupState;
    private readonly ICredentialValidationGateway _credentials;
    private readonly ILoginAttemptStore _attempts;
    private readonly ILoginAuditSink _audit;

    public LoginHandler(ISetupStateStore setupState, ICredentialValidationGateway credentials, ILoginAttemptStore attempts, ILoginAuditSink audit)
    {
        _setupState = setupState;
        _credentials = credentials;
        _attempts = attempts;
        _audit = audit;
    }

    public async Task<LoginResult> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var setup = await _setupState.GetAsync(cancellationToken);
        if (!setup.IsCompleted || string.IsNullOrWhiteSpace(setup.AcceptedSid))
        {
            return await FailAsync(command.UserName, command.CorrelationId, "login.setup.unavailable", cancellationToken);
        }

        if ((await _attempts.GetStatusAsync(command.UserName, cancellationToken)).IsLockedOut)
        {
            return await FailAsync(command.UserName, command.CorrelationId, "login.locked", cancellationToken);
        }

        var result = await _credentials.ValidateAsync(command.UserName, command.SecretReference, command.CorrelationId, cancellationToken);
        if (!result.Succeeded || string.IsNullOrWhiteSpace(result.Sid) || !string.Equals(result.Sid, setup.AcceptedSid, StringComparison.Ordinal))
        {
            return await FailAsync(command.UserName, command.CorrelationId, "login.denied", cancellationToken);
        }

        await _attempts.RecordSuccessAsync(command.UserName, cancellationToken);
        await _audit.WriteAsync("succeeded", command.CorrelationId, cancellationToken);
        return LoginResult.Success(result.Sid);
    }

    private async Task<LoginResult> FailAsync(string userName, string correlationId, string errorCode, CancellationToken cancellationToken)
    {
        await _attempts.RecordFailureAsync(userName, cancellationToken);
        await _audit.WriteAsync("denied", correlationId, cancellationToken);
        return LoginResult.Failure(errorCode);
    }
}
