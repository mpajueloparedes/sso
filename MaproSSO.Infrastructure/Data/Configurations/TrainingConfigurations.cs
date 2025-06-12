using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Trainings;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class TrainingConfiguration : IEntityTypeConfiguration<Training>
{
    public void Configure(EntityTypeBuilder<Training> builder)
    {
        builder.ToTable("Trainings", "SSO");

        builder.HasKey(e => e.TrainingId);

        builder.Property(e => e.TrainingCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.TrainingType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Mode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.ExternalInstructor)
            .HasMaxLength(200);

        builder.Property(e => e.Location)
            .HasMaxLength(200);

        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Scheduled");

        builder.Property(e => e.MaterialUrl)
            .HasMaxLength(1000);

        builder.HasOne(e => e.Instructor)
            .WithMany()
            .HasForeignKey(e => e.InstructorUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Area)
            .WithMany()
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.TrainingCode })
            .IsUnique()
            .HasDatabaseName("UQ_Trainings_Code");

        builder.HasIndex(e => new { e.TenantId, e.AreaId })
            .HasDatabaseName("IX_Trainings_TenantArea");
    }
}

public class TrainingParticipantConfiguration : IEntityTypeConfiguration<TrainingParticipant>
{
    public void Configure(EntityTypeBuilder<TrainingParticipant> builder)
    {
        builder.ToTable("TrainingParticipants", "SSO");

        builder.HasKey(e => e.ParticipantId);

        builder.Property(e => e.AttendanceStatus)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Registered");

        builder.Property(e => e.Score)
            .HasPrecision(5, 2);

        builder.Property(e => e.CertificateUrl)
            .HasMaxLength(1000);

        builder.Property(e => e.Comments)
            .HasMaxLength(500);

        builder.HasOne(e => e.Training)
            .WithMany(t => t.Participants)
            .HasForeignKey(e => e.TrainingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TrainingId, e.UserId })
            .IsUnique()
            .HasDatabaseName("UQ_TrainingParticipants_User");
    }
}