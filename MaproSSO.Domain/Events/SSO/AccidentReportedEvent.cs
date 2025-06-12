using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;

namespace MaproSSO.Domain.Events.SSO
{
    public class AccidentReportedEvent : IDomainEvent
    {
        public Guid AccidentId { get; }
        public Guid TenantId { get; }
        public string AccidentCode { get; }
        public AccidentType Type { get; }
        public AccidentSeverity Severity { get; }
        public DateTime OccurredOn { get; }

        public AccidentReportedEvent(Guid accidentId, Guid tenantId, string accidentCode,
            AccidentType type, AccidentSeverity severity)
        {
            AccidentId = accidentId;
            TenantId = tenantId;
            AccidentCode = accidentCode;
            Type = type;
            Severity = severity;
            OccurredOn = DateTime.UtcNow;
        }
    }
}