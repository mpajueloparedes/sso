using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.SSO
{
    public class InspectionCompletedEvent : IDomainEvent
    {
        public Guid InspectionId { get; }
        public Guid TenantId { get; }
        public string InspectionCode { get; }
        public DateTime CompletedAt { get; }
        public DateTime OccurredOn { get; }

        public InspectionCompletedEvent(Guid inspectionId, Guid tenantId, string inspectionCode, DateTime completedAt)
        {
            InspectionId = inspectionId;
            TenantId = tenantId;
            InspectionCode = inspectionCode;
            CompletedAt = completedAt;
            OccurredOn = DateTime.UtcNow;
        }
    }
}