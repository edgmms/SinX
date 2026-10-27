namespace SinX.Windows.Authentication;

public interface IWindowsCredentialValidator
{
    Task<WindowsCredentialValidationResult> ValidateAsync(string userName, string password, CancellationToken cancellationToken);
}

public sealed record WindowsCredentialValidationResult(bool Succeeded, string? Sid, string ErrorCode);
