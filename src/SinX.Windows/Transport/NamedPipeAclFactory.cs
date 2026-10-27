using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace SinX.Windows.Transport;

/// <summary>Builds the ADR-0002 ACL for the local Web-to-Worker pipe on Windows.</summary>
public static class NamedPipeAclFactory
{
    public static PipeSecurity Create(string webServiceSid, string workerServiceSid)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Named-pipe ACLs require Windows.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(webServiceSid);
        ArgumentException.ThrowIfNullOrWhiteSpace(workerServiceSid);

        var security = new PipeSecurity();
        security.SetAccessRuleProtection(isProtected: true, preserveInheritance: false);
        security.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(webServiceSid),
            PipeAccessRights.ReadWrite,
            AccessControlType.Allow));
        security.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(workerServiceSid),
            PipeAccessRights.FullControl,
            AccessControlType.Allow));
        return security;
    }
}
