namespace SinX.Application.Features.Setup.CompleteSetup;

public sealed record CompleteSetupCommand(string UserName, string SecretReference, string CorrelationId);
