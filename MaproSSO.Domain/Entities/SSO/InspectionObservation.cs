using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class InspectionObservation : BaseAuditableEntity
    {
        private readonly List<ObservationImage> _images = new();
        private readonly List<ObservationEvidence> _evidences = new();

        public Guid InspectionId { get; private set; }
        public string ObservationCode { get; private set; }
        public string Description { get; private set; }
        public string Type { get; private set; } // Safety, Quality, Environment, Compliance
        public Severity Severity { get; private set; }
        public Guid ResponsibleUserId { get; private set; }
        public DateTime DueDate { get; private set; }
        public ActionStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public IReadOnlyCollection<ObservationImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<ObservationEvidence> Evidences => _evidences.AsReadOnly();

        private InspectionObservation() { }

        public static InspectionObservation Create(
            Guid inspectionId,
            string observationCode,
            string description,
            string type,
            Severity severity,
            Guid responsibleUserId,
            DateTime dueDate,
            Guid createdBy)
        {
            var validTypes = new[] { "Safety", "Quality", "Environment", "Compliance" };
            if (!validTypes.Contains(type))
                throw new BusinessRuleValidationException("Tipo de observación inválido");

            var observation = new InspectionObservation
            {
                InspectionId = inspectionId,
                ObservationCode = observationCode.ToUpperInvariant(),
                Description = description,
                Type = type,
                Severity = severity,
                ResponsibleUserId = responsibleUserId,
                DueDate = dueDate,
                Status = ActionStatus.Pending
            };

            observation.SetCreatedInfo(createdBy);
            return observation;
        }

        public void AddImage(string imageUrl, string description = null)
        {
            if (_images.Count >= 3)
                throw new BusinessRuleValidationException("No se pueden agregar más de 3 imágenes por observación");

            var sortOrder = _images.Count;
            _images.Add(ObservationImage.Create(Id, imageUrl, description, sortOrder));
        }

        public void AddEvidence(string description, string evidenceUrl, Guid uploadedBy)
        {
            var evidence = ObservationEvidence.Create(Id, description, evidenceUrl, uploadedBy);
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
                throw new BusinessRuleValidationException("La observación ya está completada");

            if (!_evidences.Any())
                throw new BusinessRuleValidationException("Se requiere al menos una evidencia para completar la observación");

            Status = ActionStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            SetUpdatedInfo(completedBy);
        }
    }
}