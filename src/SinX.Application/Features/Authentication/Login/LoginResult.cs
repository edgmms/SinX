namespace SinX.Application.Features.Authentication.Login;

public sealed record LoginResult(bool Succeeded, string? Sid, string ErrorCode)
{
    public static LoginResult Failure(string errorCode) => new(false, null, errorCode);
    public static LoginResult Success(string sid) => new(true, sid, "login.succeeded");
}
