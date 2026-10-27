using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SinX.Worker.Transport;

/// <summary>Fails closed until installer configures distinct Web/Worker identities and pipe ACL evidence.</summary>
public sealed class WorkerTransportHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var configured = configuration.GetValue<bool>("WorkerTransport:AclProvisioned");
        return Task.FromResult(configured
            ? HealthCheckResult.Healthy("Named-pipe ACL provisioning declared.")
            : HealthCheckResult.Unhealthy("Named-pipe ACL provisioning is required."));
    }
}
