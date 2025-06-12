using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Domain.Entities.Tenants;
using MaproSSO.Domain.Entities.Subscriptions;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Entities.Areas;
using MaproSSO.Domain.Entities.Announcements;
using MaproSSO.Domain.Entities.Pillars;
using MaproSSO.Domain.Entities.Inspections;
using MaproSSO.Domain.Entities.Audits;
using MaproSSO.Domain.Entities.Accidents;
using MaproSSO.Domain.Entities.Trainings;
using MaproSSO.Domain.Entities.Audit;
using MaproSSO.Infrastructure.Data.Configurations;

namespace MaproSSO.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Tenant Management
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantSetting> TenantSettings => Set<TenantSetting>();

    // Subscription Management
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<PlanFeature> PlanFeatures => Set<PlanFeature>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SubscriptionHistory> SubscriptionHistory => Set<SubscriptionHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<FeatureUsage> FeatureUsages => Set<FeatureUsage>();

    // Security
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordHistory> PasswordHistory => Set<PasswordHistory>();
    public DbSet<TwoFactorAuth> TwoFactorAuths => Set<TwoFactorAuth>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    // SSO Areas
    public DbSet<Area> Areas => Set<Area>();
    public DbSet<AreaUser> AreaUsers => Set<AreaUser>();
    public DbSet<ContractorCompany> ContractorCompanies => Set<ContractorCompany>();

    // SSO Announcements
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<AnnouncementImage> AnnouncementImages => Set<AnnouncementImage>();
    public DbSet<CorrectiveAction> CorrectiveActions => Set<CorrectiveAction>();
    public DbSet<ActionEvidence> ActionEvidences => Set<ActionEvidence>();

    // SSO Pillars
    public DbSet<Pillar> Pillars => Set<Pillar>();
    public DbSet<DocumentFolder> DocumentFolders => Set<DocumentFolder>();
    public DbSet<Document> Documents => Set<Document>();

    // SSO Inspections
    public DbSet<InspectionProgram> InspectionPrograms => Set<InspectionProgram>();
    public DbSet<InspectionProgramDetail> InspectionProgramDetails => Set<InspectionProgramDetail>();
    public DbSet<Inspection> Inspections => Set<Inspection>();
    public DbSet<InspectionObservation> InspectionObservations => Set<InspectionObservation>();
    public DbSet<ObservationImage> ObservationImages => Set<ObservationImage>();
    public DbSet<ObservationEvidence> ObservationEvidences => Set<ObservationEvidence>();

    // SSO Audits
    public DbSet<AuditProgram> AuditPrograms => Set<AuditProgram>();
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<AuditCategory> AuditCategories => Set<AuditCategory>();
    public DbSet<AuditCriteria> AuditCriteria => Set<AuditCriteria>();
    public DbSet<AuditEvaluation> AuditEvaluations => Set<AuditEvaluation>();
    public DbSet<AuditEvidence> AuditEvidences => Set<AuditEvidence>();

    // SSO Accidents
    public DbSet<Accident> Accidents => Set<Accident>();
    public DbSet<AccidentPerson> AccidentPeople => Set<AccidentPerson>();
    public DbSet<AccidentImage> AccidentImages => Set<AccidentImage>();

    // SSO Trainings
    public DbSet<Training> Trainings => Set<Training>();
    public DbSet<TrainingParticipant> TrainingParticipants => Set<TrainingParticipant>();

    // Audit Logs
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Apply existing configurations
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new TenantSettingConfiguration());
        modelBuilder.ApplyConfiguration(new PlanConfiguration());
        modelBuilder.ApplyConfiguration(new PlanFeatureConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new AreaConfiguration());
        modelBuilder.ApplyConfiguration(new AnnouncementConfiguration());
        modelBuilder.ApplyConfiguration(new PillarConfiguration());

        // Apply new SSO configurations
        modelBuilder.ApplyConfiguration(new InspectionProgramConfiguration());
        modelBuilder.ApplyConfiguration(new InspectionConfiguration());
        modelBuilder.ApplyConfiguration(new InspectionObservationConfiguration());
        modelBuilder.ApplyConfiguration(new AuditProgramConfiguration());
        modelBuilder.ApplyConfiguration(new AuditConfiguration());
        modelBuilder.ApplyConfiguration(new AuditCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new AuditCriteriaConfiguration());
        modelBuilder.ApplyConfiguration(new AuditEvaluationConfiguration());
        modelBuilder.ApplyConfiguration(new AccidentConfiguration());
        modelBuilder.ApplyConfiguration(new AccidentPersonConfiguration());
        modelBuilder.ApplyConfiguration(new AccidentImageConfiguration());
        modelBuilder.ApplyConfiguration(new TrainingConfiguration());
        modelBuilder.ApplyConfiguration(new TrainingParticipantConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Apply audit information before saving
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseAuditableEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseAuditableEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTime.UtcNow;
                // entity.CreatedBy should be set by the application layer
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTime.UtcNow;
                // entity.UpdatedBy should be set by the application layer
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}