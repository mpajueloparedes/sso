using MaproSSO.Domain.Entities.Announcements;
using MaproSSO.Domain.Entities.Inspections;
using MaproSSO.Domain.Entities.SSO;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Domain.Specifications
{
    public class ActiveAnnouncementsSpecification : BaseSpecification<Announcement>
    {
        public ActiveAnnouncementsSpecification(Guid tenantId)
            : base(a => a.TenantId == tenantId && !a.IsDeleted)
        {
            AddInclude(a => a.Area);
            AddInclude(a => a.Images);
            AddInclude(a => a.CorrectiveActions);
            ApplyOrderByDescending(a => a.CreatedAt);
        }
    }

    public class PendingInspectionsSpecification : BaseSpecification<Inspection>
    {
        public PendingInspectionsSpecification(Guid tenantId)
            : base(i => i.TenantId == tenantId && i.Status == InspectionStatus.Pending)
        {
            AddInclude(i => i.Area);
            AddInclude(i => i.InspectorUserId);
            ApplyOrderBy(i => i.ScheduledDate);
        }
    }

    public class OverdueCorrectiveActionsSpecification : BaseSpecification<CorrectiveAction>
    {
        public OverdueCorrectiveActionsSpecification()
            : base(ca => ca.Status != ActionStatus.Completed && ca.DueDate < DateTime.UtcNow.Date)
        {
            AddInclude(ca => ca.ResponsibleUser);
            ApplyOrderBy(ca => ca.DueDate);
        }
    }
}