namespace SinX.Windows.Setup;

public sealed class WindowsSetupPlatform : IWindowsSetupPlatform
{
    public Task<WindowsSetupResult> ProvisionAsync(string userName, string password, CancellationToken cancellationToken)
    {
        if (!OperatingSystem.IsWindows())
        {
            return Task.FromResult(new WindowsSetupResult(false, null, "setup.platform.unavailable"));
        }

        return Task.FromResult(new WindowsSetupResult(false, null, "setup.platform.not_configured"));
    }

    public Task CompensateAsync(string userName, CancellationToken cancellationToken) => Task.CompletedTask;
}
