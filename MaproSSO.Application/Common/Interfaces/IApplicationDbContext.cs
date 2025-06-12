using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MaproSSO.Domain.Entities.Tenant;
using MaproSSO.Domain.Entities.Subscription;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Entities.SSO;
using System.Threading;
using System.Threading.Tasks;
using static MaproSSO.Domain.Entities.SSO.AuditCriteria;
using MaproSSO.Domain.Entities.Announcements;
using MaproSSO.Domain.Entities.Areas;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // Tenant
        DbSet<Tenant> Tenants { get; }
        DbSet<TenantSettings> TenantSettings { get; }

        // Subscription
        DbSet<Plan> Plans { get; }
        DbSet<PlanFeature> PlanFeatures { get; }
        DbSet<Subscription> Subscriptions { get; }
        DbSet<SubscriptionHistory> SubscriptionHistories { get; }
        DbSet<Payment> Payments { get; }
        DbSet<FeatureUsage> FeatureUsages { get; }

        // Security
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<UserSession> UserSessions { get; }
        DbSet<TwoFactorAuth> TwoFactorAuths { get; }
        DbSet<PasswordHistory> PasswordHistories { get; }

        // SSO - Common
        DbSet<Area> Areas { get; }
        DbSet<AreaUser> AreaUsers { get; }
        DbSet<ContractorCompany> ContractorCompanies { get; }

        // SSO - 9 Pillars
        DbSet<Pillar> Pillars { get; }
        DbSet<DocumentFolder> DocumentFolders { get; }
        DbSet<Document> Documents { get; }

        // SSO - Announcements
        DbSet<Announcement> Announcements { get; }
        DbSet<AnnouncementImage> AnnouncementImages { get; }
        DbSet<CorrectiveAction> CorrectiveActions { get; }
        DbSet<ActionEvidence> ActionEvidences { get; }

        // SSO - Inspections
        DbSet<InspectionProgram> InspectionPrograms { get; }
        DbSet<InspectionProgramDetail> InspectionProgramDetails { get; }
        DbSet<Inspection> Inspections { get; }
        DbSet<InspectionObservation> InspectionObservations { get; }
        DbSet<ObservationImage> ObservationImages { get; }
        DbSet<ObservationEvidence> ObservationEvidences { get; }

        // SSO - Audits
        DbSet<AuditProgram> AuditPrograms { get; }
        DbSet<Audit> Audits { get; }
        DbSet<AuditCategory> AuditCategories { get; }
        DbSet<AuditCriteria> AuditCriteria { get; }
        DbSet<AuditEvaluation> AuditEvaluations { get; }
        DbSet<AuditEvidence> AuditEvidences { get; }

        // SSO - Accidents
        DbSet<Accident> Accidents { get; }
        DbSet<AccidentPerson> AccidentPeople { get; }
        DbSet<AccidentImage> AccidentImages { get; }

        // SSO - Trainings
        DbSet<Training> Trainings { get; }
        DbSet<TrainingParticipant> TrainingParticipants { get; }

        // Database
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}