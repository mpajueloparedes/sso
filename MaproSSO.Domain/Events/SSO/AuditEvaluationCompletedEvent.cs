using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Events.SSO
{
    public class AuditEvaluationCompletedEvent : IDomainEvent
    {
        public Guid AuditId { get; }
        public Guid EvaluationId { get; }
        public Guid CriteriaId { get; }
        public decimal Score { get; }
        public decimal MaxScore { get; }
        public string ComplianceLevel { get; }
        public DateTime OccurredOn { get; }

        public AuditEvaluationCompletedEvent(
            Guid auditId,
            Guid evaluationId,
            Guid criteriaId,
            decimal score,
            decimal maxScore,
            string complianceLevel)
        {
            AuditId = auditId;
            EvaluationId = evaluationId;
            CriteriaId = criteriaId;
            Score = score;
            MaxScore = maxScore;
            ComplianceLevel = complianceLevel;
            OccurredOn = DateTime.UtcNow;
        }
    }
}