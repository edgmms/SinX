using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SinX.Windows.Authentication;

/// <summary>Worker-only local-account validation. Native errors deliberately never escape this boundary.</summary>
public sealed class WindowsCredentialValidator : IWindowsCredentialValidator
{
    private const int Logon32LogonInteractive = 2;
    private const int Logon32ProviderDefault = 0;

    public Task<WindowsCredentialValidationResult> ValidateAsync(string userName, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!OperatingSystem.IsWindows())
        {
            return Task.FromResult(new WindowsCredentialValidationResult(false, null, "login.platform.unavailable"));
        }
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password))
        {
            return Task.FromResult(new WindowsCredentialValidationResult(false, null, "login.denied"));
        }

        if (!LogonUser(userName, ".", password, Logon32LogonInteractive, Logon32ProviderDefault, out var token))
        {
            return Task.FromResult(new WindowsCredentialValidationResult(false, null, "login.denied"));
        }

        using (token)
        {
            try
            {
                using var identity = new System.Security.Principal.WindowsIdentity(token.DangerousGetHandle());
                var sid = identity.User?.Value;
                return Task.FromResult(string.IsNullOrWhiteSpace(sid)
                    ? new WindowsCredentialValidationResult(false, null, "login.sid.unavailable")
                    : new WindowsCredentialValidationResult(true, sid, "login.succeeded"));
            }
            catch (Exception) when (OperatingSystem.IsWindows())
            {
                return Task.FromResult(new WindowsCredentialValidationResult(false, null, "login.sid.unavailable"));
            }
        }
    }

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool LogonUser(
        string userName,
        string domain,
        string password,
        int logonType,
        int logonProvider,
        out SafeAccessTokenHandle token);
}
