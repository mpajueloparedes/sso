using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Inspections;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class InspectionProgramConfiguration : IEntityTypeConfiguration<InspectionProgram>
{
    public void Configure(EntityTypeBuilder<InspectionProgram> builder)
    {
        builder.ToTable("InspectionPrograms", "SSO");

        builder.HasKey(e => e.ProgramId);

        builder.Property(e => e.ProgramName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.ProgramType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.HasIndex(e => new { e.TenantId, e.ProgramName, e.Year })
            .IsUnique()
            .HasDatabaseName("UQ_InspectionPrograms_Name");
    }
}

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.ToTable("Inspections", "SSO");

        builder.HasKey(e => e.InspectionId);

        builder.Property(e => e.InspectionCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Pending");

        builder.Property(e => e.CompletionPercentage)
            .HasDefaultValue(0);

        builder.HasOne(e => e.Program)
            .WithMany(p => p.Inspections)
            .HasForeignKey(e => e.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Area)
            .WithMany()
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Inspector)
            .WithMany()
            .HasForeignKey(e => e.InspectorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.InspectionCode })
            .IsUnique()
            .HasDatabaseName("UQ_Inspections_Code");
    }
}

public class InspectionObservationConfiguration : IEntityTypeConfiguration<InspectionObservation>
{
    public void Configure(EntityTypeBuilder<InspectionObservation> builder)
    {
        builder.ToTable("InspectionObservations", "SSO");

        builder.HasKey(e => e.ObservationId);

        builder.Property(e => e.ObservationCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Severity)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Pending");

        builder.HasOne(e => e.Inspection)
            .WithMany(i => i.Observations)
            .HasForeignKey(e => e.InspectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.ResponsibleUser)
            .WithMany()
            .HasForeignKey(e => e.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}