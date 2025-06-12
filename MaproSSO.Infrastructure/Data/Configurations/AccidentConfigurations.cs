using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Accidents;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class AccidentConfiguration : IEntityTypeConfiguration<Accident>
{
    public void Configure(EntityTypeBuilder<Accident> builder)
    {
        builder.ToTable("Accidents", "SSO");

        builder.HasKey(e => e.AccidentId);

        builder.Property(e => e.AccidentCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Severity)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Shift)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Location)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("Reported");

        builder.HasIndex(e => new { e.TenantId, e.AccidentCode })
            .IsUnique()
            .HasDatabaseName("UQ_Accidents_Code");

        builder.HasIndex(e => new { e.TenantId, e.OccurredAt })
            .HasDatabaseName("IX_Accidents_TenantDate");
    }
}

public class AccidentPersonConfiguration : IEntityTypeConfiguration<AccidentPerson>
{
    public void Configure(EntityTypeBuilder<AccidentPerson> builder)
    {
        builder.ToTable("AccidentPeople", "SSO");

        builder.HasKey(e => e.PersonId);

        builder.Property(e => e.PersonType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.FullName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.DocumentNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Gender)
            .HasMaxLength(10);

        builder.Property(e => e.Position)
            .HasMaxLength(100);

        builder.Property(e => e.Company)
            .HasMaxLength(200);

        builder.Property(e => e.InjuryType)
            .HasMaxLength(100);

        builder.HasOne(e => e.Accident)
            .WithMany(a => a.People)
            .HasForeignKey(e => e.AccidentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Area)
            .WithMany()
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class AccidentImageConfiguration : IEntityTypeConfiguration<AccidentImage>
{
    public void Configure(EntityTypeBuilder<AccidentImage> builder)
    {
        builder.ToTable("AccidentImages", "SSO");

        builder.HasKey(e => e.ImageId);

        builder.Property(e => e.ImageUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.HasOne(e => e.Accident)
            .WithMany(a => a.Images)
            .HasForeignKey(e => e.AccidentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}