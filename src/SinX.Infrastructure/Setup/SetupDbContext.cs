using Microsoft.EntityFrameworkCore;

namespace SinX.Infrastructure.Setup;

public sealed class SetupDbContext : DbContext
{
    public SetupDbContext(DbContextOptions<SetupDbContext> options) : base(options) { }
    public DbSet<SetupStateEntity> SetupStates => Set<SetupStateEntity>();
    public DbSet<SetupAuditEntity> SetupAudits => Set<SetupAuditEntity>();
    public DbSet<TransientSecretEntity> TransientSecrets => Set<TransientSecretEntity>();
    public DbSet<LoginAttemptEntity> LoginAttempts => Set<LoginAttemptEntity>();
    public DbSet<LoginAuditEntity> LoginAudits => Set<LoginAuditEntity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SetupStateEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SetupStateEntity>().Property(x => x.Version).IsConcurrencyToken();
        modelBuilder.Entity<TransientSecretEntity>().HasKey(x => x.Reference);
        modelBuilder.Entity<SetupAuditEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<LoginAttemptEntity>().HasKey(x => x.UserName);
        modelBuilder.Entity<LoginAuditEntity>().HasKey(x => x.Id);
    }
}
public sealed class SetupStateEntity { public int Id { get; set; } = 1; public bool Completed { get; set; } public bool InProgress { get; set; } public Guid? OperationId { get; set; } public string? AcceptedSid { get; set; } public long Version { get; set; } }
public sealed class TransientSecretEntity { public string Reference { get; set; } = null!; public byte[] ProtectedValue { get; set; } = null!; public DateTimeOffset ExpiresAt { get; set; } }
public sealed class SetupAuditEntity { public Guid Id { get; set; } public string EventType { get; set; } = null!; public string Outcome { get; set; } = null!; public string CorrelationId { get; set; } = null!; public DateTimeOffset OccurredAt { get; set; } }
public sealed class LoginAttemptEntity { public string UserName { get; set; } = null!; public int FailureCount { get; set; } public DateTimeOffset WindowStartedAt { get; set; } public DateTimeOffset? LockedUntil { get; set; } }
public sealed class LoginAuditEntity { public Guid Id { get; set; } public string Outcome { get; set; } = null!; public string CorrelationId { get; set; } = null!; public DateTimeOffset OccurredAt { get; set; } }
