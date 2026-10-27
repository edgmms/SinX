namespace SinX.Infrastructure.Authentication;

public sealed class LoginSecurityOptions
{
    public const string SectionName = "LoginSecurity";
    public int MaximumFailures { get; init; } = 5;
    public TimeSpan FailureWindow { get; init; } = TimeSpan.FromMinutes(15);
    public TimeSpan LockoutDuration { get; init; } = TimeSpan.FromMinutes(15);
    public TimeSpan IdleTimeout { get; init; } = TimeSpan.FromMinutes(20);
}
