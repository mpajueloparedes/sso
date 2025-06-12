using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class Audit : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<AuditEvaluation> _evaluations = new();

        public Guid TenantId { get; private set; }
        public Guid ProgramId { get; private set; }
        public Guid AreaId { get; private set; }
        public string AuditCode { get; private set; }
        public string AuditType { get; private set; } // Internal, External, Certification
        public Guid AuditorUserId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public DateTime? ExecutedDate { get; private set; }
        public string Status { get; private set; } // Scheduled, InProgress, Completed, Cancelled
        public decimal? TotalScore { get; private set; }
        public decimal? MaxScore { get; private set; }

        public decimal CompliancePercentage => (MaxScore > 0 && TotalScore.HasValue && MaxScore.HasValue)
            ? (TotalScore.Value / MaxScore.Value) * 100
            : 0;

        public virtual AuditProgram Program { get; private set; }
        public virtual Area Area { get; private set; }
        public virtual User AuditorUser { get; private set; }
        public IReadOnlyCollection<AuditEvaluation> Evaluations => _evaluations.AsReadOnly();

        private Audit() { }

        public static Audit Create(
            Guid tenantId,
            Guid programId,
            Guid areaId,
            string auditCode,
            string auditType,
            Guid auditorUserId,
            DateTime scheduledDate,
            Guid? createdBy = null)
        {
            var validTypes = new[] { "Internal", "External", "Certification" };
            if (!validTypes.Contains(auditType))
                throw new BusinessRuleValidationException("Tipo de auditoría inválido");

            var audit = new Audit
            {
                TenantId = tenantId,
                ProgramId = programId,
                AreaId = areaId,
                AuditCode = auditCode.ToUpperInvariant(),
                AuditType = auditType,
                AuditorUserId = auditorUserId,
                ScheduledDate = scheduledDate,
                Status = "Scheduled"
            };

            if (createdBy.HasValue)
                audit.SetCreatedInfo(createdBy.Value);

            return audit;
        }

        public void Start(Guid startedBy)
        {
            if (Status != "Scheduled")
                throw new BusinessRuleValidationException("Solo las auditorías programadas pueden ser iniciadas");

            Status = "InProgress";
            ExecutedDate = DateTime.UtcNow;
            SetUpdatedInfo(startedBy);
        }

        public void AddEvaluation(
            Guid criteriaId,
            decimal score,
            decimal maxScore,
            string observations = null,
            bool evidenceRequired = false,
            Guid? evaluatedBy = null)
        {
            if (Status != "InProgress")
                throw new BusinessRuleValidationException("Solo se pueden agregar evaluaciones a auditorías en progreso");

            if (_evaluations.Any(e => e.CriteriaId == criteriaId))
                throw new BusinessRuleValidationException("Ya existe una evaluación para este criterio");

            var evaluation = AuditEvaluation.Create(
                Id, criteriaId, score, maxScore, observations, evidenceRequired, evaluatedBy ?? AuditorUserId);

            _evaluations.Add(evaluation);
            UpdateScores();
        }

        public void UpdateEvaluation(
            Guid criteriaId,
            decimal score,
            string observations,
            Guid updatedBy)
        {
            var evaluation = _evaluations.FirstOrDefault(e => e.CriteriaId == criteriaId);
            if (evaluation == null)
                throw new EntityNotFoundException("Evaluación no encontrada");

            evaluation.UpdateScore(score, observations);
            UpdateScores();
            SetUpdatedInfo(updatedBy);
        }

        private void UpdateScores()
        {
            if (_evaluations.Any())
            {
                TotalScore = _evaluations.Sum(e => e.Score);
                MaxScore = _evaluations.Sum(e => e.MaxScore);
            }
            else
            {
                TotalScore = null;
                MaxScore = null;
            }
        }

        public void Complete(Guid completedBy)
        {
            if (Status != "InProgress")
                throw new BusinessRuleValidationException("Solo las auditorías en progreso pueden ser completadas");

            if (!_evaluations.Any())
                throw new BusinessRuleValidationException("Debe haber al menos una evaluación para completar la auditoría");

            Status = "Completed";
            SetUpdatedInfo(completedBy);
        }

        public void Cancel(string reason, Guid cancelledBy)
        {
            if (Status == "Completed" || Status == "Cancelled")
                throw new BusinessRuleValidationException("No se puede cancelar una auditoría completada o ya cancelada");

            Status = "Cancelled";
            SetUpdatedInfo(cancelledBy);
        }
    }
}