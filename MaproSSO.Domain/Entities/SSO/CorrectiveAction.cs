using MaproSSO.Domain.Common;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class CorrectiveAction : BaseAuditableEntity
    {
        private readonly List<ActionEvidence> _evidences = new();

        public Guid AnnouncementId { get; private set; }
        public string Description { get; private set; }
        public Guid ResponsibleUserId { get; private set; }
        public DateTime DueDate { get; private set; }
        public ActionStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public virtual User ResponsibleUser { get; private set; }
        public IReadOnlyCollection<ActionEvidence> Evidences => _evidences.AsReadOnly();

        private CorrectiveAction() { }

        public static CorrectiveAction Create(
            Guid announcementId,
            string description,
            Guid responsibleUserId,
            DateTime dueDate,
            Guid createdBy)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("La descripción es requerida");

            if (dueDate < DateTime.UtcNow.Date)
                throw new BusinessRuleValidationException("La fecha límite no puede ser en el pasado");

            var action = new CorrectiveAction
            {
                AnnouncementId = announcementId,
                Description = description,
                ResponsibleUserId = responsibleUserId,
                DueDate = dueDate,
                Status = ActionStatus.Pending
            };

            action.SetCreatedInfo(createdBy);
            return action;
        }

        public void Update(string description, DateTime dueDate, Guid updatedBy)
        {
            if (Status == ActionStatus.Completed)
                throw new BusinessRuleValidationException("No se puede editar una acción completada");

            Description = description;
            DueDate = dueDate;
            SetUpdatedInfo(updatedBy);
        }

        public void AddEvidence(string description, string evidenceUrl, Guid uploadedBy)
        {
            var evidence = ActionEvidence.Create(Id, description, evidenceUrl, uploadedBy);
            _evidences.Add(evidence);

            if (Status == ActionStatus.Pending)
            {
                Status = ActionStatus.InProgress;
                SetUpdatedInfo(uploadedBy);
            }
        }

        public void Complete(Guid completedBy)
        {
            if (Status == ActionStatus.Completed)
                throw new BusinessRuleValidationException("La acción ya está completada");

            if (!_evidences.Any())
                throw new BusinessRuleValidationException("Se requiere al menos una evidencia para completar la acción");

            Status = ActionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            SetUpdatedInfo(completedBy);
        }

        public bool IsOverdue => Status != ActionStatus.Completed && DueDate < DateTime.UtcNow.Date;

        public int DaysUntilDue => Status != ActionStatus.Completed
            ? (int)(DueDate - DateTime.UtcNow.Date).TotalDays
            : 0;
    }
}