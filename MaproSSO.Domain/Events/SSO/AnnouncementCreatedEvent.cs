using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Domain.Events.SSO
{
    public class AnnouncementCreatedEvent : IDomainEvent
    {
        public Guid AnnouncementId { get; }
        public Guid TenantId { get; }
        public string Title { get; }
        public Severity Severity { get; }
        public DateTime OccurredOn { get; }

        public AnnouncementCreatedEvent(Guid announcementId, Guid tenantId, string title, Severity severity)
        {
            AnnouncementId = announcementId;
            TenantId = tenantId;
            Title = title;
            Severity = severity;
            OccurredOn = DateTime.UtcNow;
        }
    }
}