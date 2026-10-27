namespace SinX.Application.Features.Setup.CompleteSetup;

public sealed record CompleteSetupResult(bool Succeeded, bool AlreadyCompleted, string? ErrorCode)
{
    public static CompleteSetupResult Success() => new(true, false, null);
    public static CompleteSetupResult Failure(string errorCode) => new(false, false, errorCode);
    public static CompleteSetupResult Completed() => new(false, true, null);
}
