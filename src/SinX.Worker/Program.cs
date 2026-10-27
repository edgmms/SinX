using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Microsoft.Extensions.Hosting.WindowsServices;
using SinX.Windows.Transport;
using SinX.Worker.Operations;
using SinX.Worker.Transport;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWindowsService();
if (!OperatingSystem.IsWindows())
{
    throw new PlatformNotSupportedException("SinX.Worker named-pipe host requires Windows.");
}

builder.WebHost.UseNamedPipes(options =>
{
    var webSid = builder.Configuration["WorkerTransport:WebServiceSid"];
    var workerSid = builder.Configuration["WorkerTransport:WorkerServiceSid"];
    if (!string.IsNullOrWhiteSpace(webSid) && !string.IsNullOrWhiteSpace(workerSid))
    {
        options.PipeSecurity = NamedPipeAclFactory.Create(webSid, workerSid);
    }
});
builder.WebHost.ConfigureKestrel(options =>
{
    // Kestrel named pipes are local-only; installer must apply the ADR-0002 pipe ACL.
    options.ListenNamedPipe("sinx-worker-v1", listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});
builder.Services.AddGrpc();
builder.Services.AddHealthChecks().AddCheck<WorkerTransportHealthCheck>("worker-transport");

var app = builder.Build();
app.MapGrpcService<WorkerDispatchGrpcService>();
app.MapHealthChecks("/health");
app.Run();

public partial class Program;
