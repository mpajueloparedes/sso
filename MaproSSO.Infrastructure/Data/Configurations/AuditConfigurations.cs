using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Audits;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class AuditProgramConfiguration : IEntityTypeConfiguration<AuditProgram>
{
    public void Configure(EntityTypeBuilder<AuditProgram> builder)
    {
        builder.ToTable("AuditPrograms", "SSO");

        builder.HasKey(e => e.ProgramId);

        builder.Property(e => e.ProgramName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Standard)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.ProgramName, e.Year })
            .IsUnique()
            .HasDatabaseName("UQ_AuditPrograms_Name");
    }
}

public class AuditConfiguration : IEntityTypeConfiguration<Audit>
{
    public void Configure(EntityTypeBuilder<Audit> builder)
    {
        builder.ToTable("Audits", "SSO");

        builder.HasKey(e => e.AuditId);

        builder.Property(e => e.AuditCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.AuditType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Scheduled");

        builder.Property(e => e.TotalScore)
            .HasPrecision(5, 2);

        builder.Property(e => e.MaxScore)
            .HasPrecision(5, 2);

        builder.HasOne(e => e.Program)
            .WithMany(p => p.Audits)
            .HasForeignKey(e => e.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Area)
            .WithMany()
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Auditor)
            .WithMany()
            .HasForeignKey(e => e.AuditorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.AuditCode })
            .IsUnique()
            .HasDatabaseName("UQ_Audits_Code");
    }
}

public class AuditCategoryConfiguration : IEntityTypeConfiguration<AuditCategory>
{
    public void Configure(EntityTypeBuilder<AuditCategory> builder)
    {
        builder.ToTable("AuditCategories", "SSO");

        builder.HasKey(e => e.CategoryId);

        builder.Property(e => e.CategoryName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.CategoryCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.CategoryCode)
            .IsUnique()
            .HasDatabaseName("UQ_AuditCategories_Code");
    }
}

public class AuditCriteriaConfiguration : IEntityTypeConfiguration<AuditCriteria>
{
    public void Configure(EntityTypeBuilder<AuditCriteria> builder)
    {
        builder.ToTable("AuditCriteria", "SSO");

        builder.HasKey(e => e.CriteriaId);

        builder.Property(e => e.CriteriaCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.MaxScore)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Criteria)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.CategoryId, e.CriteriaCode })
            .IsUnique()
            .HasDatabaseName("UQ_AuditCriteria_Code");
    }
}


public class AuditEvaluationConfiguration : IEntityTypeConfiguration<AuditEvaluation>
{
    public void Configure(EntityTypeBuilder<AuditEvaluation> builder)
    {
        builder.ToTable("AuditEvaluations", "SSO");

        builder.HasKey(e => e.EvaluationId);

        builder.Property(e => e.Score)
            .HasPrecision(5, 2)
            .IsRequired();

        builder.HasOne(e => e.Audit)
            .WithMany(a => a.Evaluations)
            .HasForeignKey(e => e.AuditId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Criteria)
            .WithMany(c => c.Evaluations)
            .HasForeignKey(e => e.CriteriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.EvaluatedByUser)
            .WithMany()
            .HasForeignKey(e => e.EvaluatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.AuditId, e.CriteriaId })
            .IsUnique()
            .HasDatabaseName("UQ_AuditEvaluations_Criteria");
    }
}