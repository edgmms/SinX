namespace SinX.Application.Features.Authentication.Login;

public sealed record LoginCommand(string UserName, string SecretReference, string CorrelationId);
