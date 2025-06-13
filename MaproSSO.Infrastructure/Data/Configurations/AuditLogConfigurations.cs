using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Audit;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "Audit");

        builder.HasKey(e => e.AuditId);

        builder.Property(e => e.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EntityId)
            .HasMaxLength(100);

        builder.Property(e => e.UserName)
            .HasMaxLength(100);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.UserAgent)
            .HasMaxLength(500);

        builder.Property(e => e.Success)
            .HasDefaultValue(true);

        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.Timestamp })
            .HasDatabaseName("IX_AuditLogs_TenantUser");

        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("IX_AuditLogs_Entity");

        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_AuditLogs_Timestamp");

        builder.HasIndex(e => e.Action)
            .HasDatabaseName("IX_AuditLogs_Action");
    }
}

public class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
{
    public void Configure(EntityTypeBuilder<AccessLog> builder)
    {
        builder.ToTable("AccessLogs", "Audit");

        builder.HasKey(e => e.LogId);

        builder.Property(e => e.Action)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.UserName)
            .HasMaxLength(100);

        builder.Property(e => e.IpAddress)
            .HasMaxLength(50);

        builder.Property(e => e.Location)
            .HasMaxLength(200);

        builder.Property(e => e.DeviceInfo)
            .HasMaxLength(500);

        builder.Property(e => e.Browser)
            .HasMaxLength(100);

        builder.Property(e => e.OperatingSystem)
            .HasMaxLength(100);

        builder.Property(e => e.Success)
            .HasDefaultValue(true);

        builder.Property(e => e.FailureReason)
            .HasMaxLength(500);

        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Tenant)
            .WithMany()
            .HasForeignKey(e => e.TenantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => new { e.TenantId, e.UserId, e.Timestamp })
            .HasDatabaseName("IX_AccessLogs_TenantUser");

        builder.HasIndex(e => e.IpAddress)
            .HasDatabaseName("IX_AccessLogs_IpAddress");

        builder.HasIndex(e => e.Timestamp)
            .HasDatabaseName("IX_AccessLogs_Timestamp");

        builder.HasIndex(e => new { e.Action, e.Success })
            .HasDatabaseName("IX_AccessLogs_ActionSuccess");
    }
}