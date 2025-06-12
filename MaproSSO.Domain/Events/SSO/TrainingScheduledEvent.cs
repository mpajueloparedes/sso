using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.SSO
{
    public class TrainingScheduledEvent : IDomainEvent
    {
        public Guid TrainingId { get; }
        public Guid TenantId { get; }
        public string Title { get; }
        public DateTime ScheduledDate { get; }
        public int MaxParticipants { get; }
        public DateTime OccurredOn { get; }

        public TrainingScheduledEvent(Guid trainingId, Guid tenantId, string title,
            DateTime scheduledDate, int maxParticipants)
        {
            TrainingId = trainingId;
            TenantId = tenantId;
            Title = title;
            ScheduledDate = scheduledDate;
            MaxParticipants = maxParticipants;
            OccurredOn = DateTime.UtcNow;
        }
    }
}