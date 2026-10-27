using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SinX.Application.Abstractions.Authentication;
using SinX.Infrastructure.Setup;

namespace SinX.Infrastructure.Authentication;

public sealed class EfLoginSecurityStore : ILoginAttemptStore, ILoginAuditSink
{
    private readonly SetupDbContext _db;
    private readonly TimeProvider _clock;
    private readonly LoginSecurityOptions _options;

    public EfLoginSecurityStore(SetupDbContext db, TimeProvider clock, IOptions<LoginSecurityOptions> options)
    {
        _db = db;
        _clock = clock;
        _options = options.Value;
    }

    public async Task<LoginAttemptStatus> GetStatusAsync(string userName, CancellationToken cancellationToken)
    {
        var attempt = await _db.LoginAttempts.SingleOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        return new LoginAttemptStatus(attempt?.LockedUntil is { } lockedUntil && lockedUntil > _clock.GetUtcNow());
    }

    public async Task RecordFailureAsync(string userName, CancellationToken cancellationToken)
    {
        var now = _clock.GetUtcNow();
        var attempt = await _db.LoginAttempts.SingleOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (attempt is null)
        {
            attempt = new LoginAttemptEntity { UserName = userName, WindowStartedAt = now, FailureCount = 0 };
            _db.LoginAttempts.Add(attempt);
        }
        if (attempt.WindowStartedAt.Add(_options.FailureWindow) <= now)
        {
            attempt.WindowStartedAt = now;
            attempt.FailureCount = 0;
            attempt.LockedUntil = null;
        }
        attempt.FailureCount++;
        if (attempt.FailureCount >= _options.MaximumFailures)
        {
            attempt.LockedUntil = now.Add(_options.LockoutDuration);
        }
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordSuccessAsync(string userName, CancellationToken cancellationToken)
    {
        var attempt = await _db.LoginAttempts.SingleOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        if (attempt is not null)
        {
            _db.LoginAttempts.Remove(attempt);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task WriteAsync(string outcome, string correlationId, CancellationToken cancellationToken)
    {
        _db.LoginAudits.Add(new LoginAuditEntity
        {
            Id = Guid.NewGuid(),
            Outcome = outcome,
            CorrelationId = correlationId,
            OccurredAt = _clock.GetUtcNow()
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}
