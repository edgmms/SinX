namespace SinX.Contracts.Setup;

public sealed record SetupProvisioningRequest(
    int ContractVersion,
    WorkerOperation Operation,
    Guid OperationId,
    string Actor,
    string Scope,
    string Target,
    string UserName,
    string SecretReference,
    string CorrelationId);

public sealed record SetupProvisioningResponse(
    bool Succeeded,
    string? AcceptedSid,
    string ErrorCode);

public enum WorkerOperation
{
    SetupProvision = 1,
    LoginValidate = 2
}
