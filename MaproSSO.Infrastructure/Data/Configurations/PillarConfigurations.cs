using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MaproSSO.Domain.Entities.Pillars;

namespace MaproSSO.Infrastructure.Data.Configurations;

public class PillarConfiguration : IEntityTypeConfiguration<Pillar>
{
    public void Configure(EntityTypeBuilder<Pillar> builder)
    {
        builder.ToTable("Pillars", "SSO");

        builder.HasKey(e => e.PillarId);

        builder.Property(e => e.PillarName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.PillarCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Icon)
            .HasMaxLength(100);

        builder.Property(e => e.Color)
            .HasMaxLength(10);

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.Property(e => e.SortOrder)
            .HasDefaultValue(0);

        builder.HasIndex(e => new { e.TenantId, e.PillarCode })
            .IsUnique()
            .HasDatabaseName("UQ_Pillars_Code");

        builder.HasIndex(e => e.TenantId)
            .HasDatabaseName("IX_Pillars_TenantId");
    }
}

public class DocumentFolderConfiguration : IEntityTypeConfiguration<DocumentFolder>
{
    public void Configure(EntityTypeBuilder<DocumentFolder> builder)
    {
        builder.ToTable("DocumentFolders", "SSO");

        builder.HasKey(e => e.FolderId);

        builder.Property(e => e.FolderName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Path)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.IsSystemFolder)
            .HasDefaultValue(false);

        builder.HasOne(e => e.Pillar)
            .WithMany(p => p.DocumentFolders)
            .HasForeignKey(e => e.PillarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Area)
            .WithMany()
            .HasForeignKey(e => e.AreaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentFolder)
            .WithMany(f => f.SubFolders)
            .HasForeignKey(e => e.ParentFolderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.PillarId, e.AreaId })
            .HasDatabaseName("IX_DocumentFolders_TenantPillarArea");

        builder.HasIndex(e => e.Path)
            .HasDatabaseName("IX_DocumentFolders_Path");
    }
}

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable("Documents", "SSO");

        builder.HasKey(e => e.DocumentId);

        builder.Property(e => e.FileName)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.FileExtension)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.StorageUrl)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(e => e.Version)
            .HasDefaultValue(1);

        builder.Property(e => e.IsCurrentVersion)
            .HasDefaultValue(true);

        builder.Property(e => e.Tags)
            .HasMaxLength(500);

        builder.HasOne(e => e.Folder)
            .WithMany(f => f.Documents)
            .HasForeignKey(e => e.FolderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ParentDocument)
            .WithMany(d => d.DocumentVersions)
            .HasForeignKey(e => e.ParentDocumentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedByUser)
            .WithMany()
            .HasForeignKey(e => e.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedByUser)
            .WithMany()
            .HasForeignKey(e => e.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.DeletedByUser)
            .WithMany()
            .HasForeignKey(e => e.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => new { e.TenantId, e.FolderId })
            .HasDatabaseName("IX_Documents_TenantFolder");

        builder.HasIndex(e => e.FileName)
            .HasDatabaseName("IX_Documents_FileName");

        builder.HasIndex(e => e.FileExtension)
            .HasDatabaseName("IX_Documents_FileExtension");

        builder.HasIndex(e => new { e.IsCurrentVersion, e.DeletedAt })
            .HasDatabaseName("IX_Documents_CurrentVersion");

        builder.HasIndex(e => e.Tags)
            .HasDatabaseName("IX_Documents_Tags");

        // Global query filter to exclude soft-deleted documents by default
        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}