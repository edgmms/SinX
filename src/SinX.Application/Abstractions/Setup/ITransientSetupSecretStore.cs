namespace SinX.Application.Abstractions.Setup;

public interface ITransientSetupSecretStore
{
    Task<string> StoreAsync(string password, CancellationToken cancellationToken);
    Task<string?> ConsumeAsync(string secretReference, CancellationToken cancellationToken);
}
