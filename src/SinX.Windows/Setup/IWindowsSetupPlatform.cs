namespace SinX.Windows.Setup;

public interface IWindowsSetupPlatform
{
    Task<WindowsSetupResult> ProvisionAsync(string userName, string password, CancellationToken cancellationToken);
    Task CompensateAsync(string userName, CancellationToken cancellationToken);
}

public sealed record WindowsSetupResult(bool Succeeded, string? Sid, string ErrorCode);
