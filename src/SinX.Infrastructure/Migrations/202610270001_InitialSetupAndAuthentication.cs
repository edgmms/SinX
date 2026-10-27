using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using SinX.Infrastructure.Setup;

#nullable disable

namespace SinX.Infrastructure.Migrations;

[DbContext(typeof(SetupDbContext))]
[Migration("202610270001_InitialSetupAndAuthentication")]
public partial class InitialSetupAndAuthentication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SetupStates",
            columns: table => new
            {
                Id = table.Column<int>(nullable: false),
                Completed = table.Column<bool>(nullable: false),
                InProgress = table.Column<bool>(nullable: false),
                OperationId = table.Column<Guid>(nullable: true),
                AcceptedSid = table.Column<string>(nullable: true),
                Version = table.Column<long>(nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_SetupStates", x => x.Id));
        migrationBuilder.CreateTable(
            name: "TransientSecrets",
            columns: table => new { Reference = table.Column<string>(nullable: false), ProtectedValue = table.Column<byte[]>(nullable: false), ExpiresAt = table.Column<DateTimeOffset>(nullable: false) },
            constraints: table => table.PrimaryKey("PK_TransientSecrets", x => x.Reference));
        migrationBuilder.CreateTable(
            name: "SetupAudits",
            columns: table => new { Id = table.Column<Guid>(nullable: false), EventType = table.Column<string>(nullable: false), Outcome = table.Column<string>(nullable: false), CorrelationId = table.Column<string>(nullable: false), OccurredAt = table.Column<DateTimeOffset>(nullable: false) },
            constraints: table => table.PrimaryKey("PK_SetupAudits", x => x.Id));
        migrationBuilder.CreateTable(
            name: "LoginAttempts",
            columns: table => new { UserName = table.Column<string>(nullable: false), FailureCount = table.Column<int>(nullable: false), WindowStartedAt = table.Column<DateTimeOffset>(nullable: false), LockedUntil = table.Column<DateTimeOffset>(nullable: true) },
            constraints: table => table.PrimaryKey("PK_LoginAttempts", x => x.UserName));
        migrationBuilder.CreateTable(
            name: "LoginAudits",
            columns: table => new { Id = table.Column<Guid>(nullable: false), Outcome = table.Column<string>(nullable: false), CorrelationId = table.Column<string>(nullable: false), OccurredAt = table.Column<DateTimeOffset>(nullable: false) },
            constraints: table => table.PrimaryKey("PK_LoginAudits", x => x.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("LoginAttempts");
        migrationBuilder.DropTable("LoginAudits");
        migrationBuilder.DropTable("SetupAudits");
        migrationBuilder.DropTable("SetupStates");
        migrationBuilder.DropTable("TransientSecrets");
    }
}
