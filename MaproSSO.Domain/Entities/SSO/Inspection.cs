using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Areas;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Events.SSO;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class Inspection : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<InspectionObservation> _observations = new();

        public Guid TenantId { get; private set; }
        public Guid ProgramId { get; private set; }
        public Guid AreaId { get; private set; }
        public string InspectionCode { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Guid InspectorUserId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public DateTime? ExecutedDate { get; private set; }
        public InspectionStatus Status { get; private set; }
        public int CompletionPercentage { get; private set; }
        public string DocumentUrl { get; private set; }

        public virtual InspectionProgram Program { get; private set; }
        public virtual Area Area { get; private set; }
        public virtual User InspectorUser { get; private set; }
        public IReadOnlyCollection<InspectionObservation> Observations => _observations.AsReadOnly();

        private Inspection() { }

        public static Inspection Create(
            Guid tenantId,
            Guid programId,
            Guid areaId,
            string inspectionCode,
            string title,
            Guid inspectorUserId,
            DateTime scheduledDate,
            string description = null,
            Guid? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(inspectionCode))
                throw new DomainException("El código de inspección es requerido");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("El título es requerido");

            if (scheduledDate < DateTime.UtcNow.Date)
                throw new BusinessRuleValidationException("La fecha programada no puede ser en el pasado");

            var inspection = new Inspection
            {
                TenantId = tenantId,
                ProgramId = programId,
                AreaId = areaId,
                InspectionCode = inspectionCode.ToUpperInvariant(),
                Title = title,
                Description = description,
                InspectorUserId = inspectorUserId,
                ScheduledDate = scheduledDate,
                Status = InspectionStatus.Pending,
                CompletionPercentage = 0
            };

            if (createdBy.HasValue)
                inspection.SetCreatedInfo(createdBy.Value);

            return inspection;
        }

        public void Start(string documentUrl, Guid startedBy)
        {
            if (Status != InspectionStatus.Pending)
                throw new BusinessRuleValidationException("Solo las inspecciones pendientes pueden ser iniciadas");

            if (string.IsNullOrWhiteSpace(documentUrl))
                throw new DomainException("El documento de inspección es requerido");

            Status = InspectionStatus.InProgress;
            ExecutedDate = DateTime.UtcNow;
            DocumentUrl = documentUrl;
            SetUpdatedInfo(startedBy);
        }

        public InspectionObservation AddObservation(
            string observationCode,
            string description,
            string type,
            Severity severity,
            Guid responsibleUserId,
            DateTime dueDate,
            Guid createdBy)
        {
            if (Status != InspectionStatus.InProgress)
                throw new BusinessRuleValidationException("Solo se pueden agregar observaciones a inspecciones en progreso");

            var observation = InspectionObservation.Create(
                Id, observationCode, description, type, severity, responsibleUserId, dueDate, createdBy);

            _observations.Add(observation);
            UpdateCompletionPercentage();

            return observation;
        }

        public void CompleteObservation(Guid observationId, Guid completedBy)
        {
            var observation = _observations.FirstOrDefault(o => o.Id == observationId);
            if (observation == null)
                throw new EntityNotFoundException("Observación no encontrada");

            observation.Complete(completedBy);
            UpdateCompletionPercentage();

            if (_observations.All(o => o.Status == ActionStatus.Completed))
            {
                Complete(completedBy);
            }
        }

        private void UpdateCompletionPercentage()
        {
            if (!_observations.Any())
            {
                CompletionPercentage = 0;
                return;
            }

            var completed = _observations.Count(o => o.Status == ActionStatus.Completed);
            CompletionPercentage = (completed * 100) / _observations.Count;
        }

        public void Complete(Guid completedBy)
        {
            if (Status == InspectionStatus.Completed)
                throw new BusinessRuleValidationException("La inspección ya está completada");

            if (_observations.Any(o => o.Status != ActionStatus.Completed))
                throw new BusinessRuleValidationException("Todas las observaciones deben estar completadas");

            Status = InspectionStatus.Completed;
            CompletionPercentage = 100;
            SetUpdatedInfo(completedBy);

            AddDomainEvent(new InspectionCompletedEvent(Id, TenantId, InspectionCode, DateTime.UtcNow));
        }

        public void Reschedule(DateTime newDate, Guid rescheduledBy)
        {
            if (Status != InspectionStatus.Pending)
                throw new BusinessRuleValidationException("Solo las inspecciones pendientes pueden ser reprogramadas");

            if (newDate < DateTime.UtcNow.Date)
                throw new BusinessRuleValidationException("La nueva fecha no puede ser en el pasado");

            ScheduledDate = newDate;
            SetUpdatedInfo(rescheduledBy);
        }
    }
}