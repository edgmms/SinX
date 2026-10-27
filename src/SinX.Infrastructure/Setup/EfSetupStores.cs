using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SinX.Application.Abstractions.Setup;

namespace SinX.Infrastructure.Setup;

public sealed class EfSetupStores : ISetupStateStore, ITransientSetupSecretStore, ISetupAuditSink
{
    private readonly SetupDbContext _db; private readonly IDataProtector _protector; private readonly TimeProvider _clock;
    public EfSetupStores(SetupDbContext db, IDataProtectionProvider protectionProvider, TimeProvider clock) { _db = db; _protector = protectionProvider.CreateProtector("SinX.Setup.TransientSecret.v1"); _clock = clock; }
    public async Task<SetupStateSnapshot> GetAsync(CancellationToken cancellationToken) { var s = await _db.SetupStates.FindAsync([1], cancellationToken) ?? new SetupStateEntity(); return new(s.Completed, s.InProgress, s.AcceptedSid); }
    public async Task<bool> TryBeginAsync(Guid operationId, CancellationToken cancellationToken) { var s = await _db.SetupStates.FindAsync([1], cancellationToken); if (s is null) { s = new SetupStateEntity { OperationId = operationId, InProgress = true }; _db.SetupStates.Add(s); } else if (s.Completed || s.InProgress) { return false; } else { s.InProgress=true; s.OperationId=operationId; s.Version++; } await _db.SaveChangesAsync(cancellationToken); return true; }
    public async Task CompleteAsync(Guid operationId, string acceptedSid, CancellationToken cancellationToken) { var s = await _db.SetupStates.FindAsync([1], cancellationToken); if (s is null || s.OperationId != operationId) { throw new InvalidOperationException(); } s.Completed=true; s.InProgress=false; s.AcceptedSid=acceptedSid; s.Version++; await _db.SaveChangesAsync(cancellationToken); }
    public async Task FailAsync(Guid operationId, string errorCode, CancellationToken cancellationToken) { var s = await _db.SetupStates.FindAsync([1], cancellationToken); if (s is not null && s.OperationId == operationId) { s.InProgress=false; s.Version++; await _db.SaveChangesAsync(cancellationToken); } }
    public async Task<string> StoreAsync(string password, CancellationToken cancellationToken) { var r=Guid.NewGuid().ToString("N"); _db.TransientSecrets.Add(new TransientSecretEntity { Reference=r, ProtectedValue=_protector.Protect(Encoding.UTF8.GetBytes(password)), ExpiresAt=_clock.GetUtcNow().AddMinutes(2) }); await _db.SaveChangesAsync(cancellationToken); return r; }
    public async Task<string?> ConsumeAsync(string secretReference, CancellationToken cancellationToken) { var s=await _db.TransientSecrets.FindAsync([secretReference],cancellationToken); if(s is null || s.ExpiresAt<_clock.GetUtcNow()) { return null; } _db.TransientSecrets.Remove(s); await _db.SaveChangesAsync(cancellationToken); return Encoding.UTF8.GetString(_protector.Unprotect(s.ProtectedValue)); }
    public async Task WriteAsync(string eventType,string outcome,string correlationId,CancellationToken cancellationToken) { _db.SetupAudits.Add(new SetupAuditEntity { Id=Guid.NewGuid(), EventType=eventType, Outcome=outcome, CorrelationId=correlationId, OccurredAt=_clock.GetUtcNow() }); await _db.SaveChangesAsync(cancellationToken); }
}
