using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Events.SSO;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class Announcement : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<AnnouncementImage> _images = new();
        private readonly List<CorrectiveAction> _correctiveActions = new();

        public Guid TenantId { get; private set; }
        public Guid AreaId { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public AnnouncementType Type { get; private set; }
        public Severity Severity { get; private set; }
        public AnnouncementStatus Status { get; private set; }
        public string Location { get; private set; }
        public DateTime ReportedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public virtual Area Area { get; private set; }
        public IReadOnlyCollection<AnnouncementImage> Images => _images.AsReadOnly();
        public IReadOnlyCollection<CorrectiveAction> CorrectiveActions => _correctiveActions.AsReadOnly();

        private Announcement() { }

        public static Announcement Create(
            Guid tenantId,
            Guid areaId,
            string title,
            string description,
            AnnouncementType type,
            Severity severity,
            string location,
            Guid createdBy)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("El título es requerido");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("La descripción es requerida");

            if (string.IsNullOrWhiteSpace(location))
                throw new DomainException("La ubicación es requerida");

            var announcement = new Announcement
            {
                TenantId = tenantId,
                AreaId = areaId,
                Title = title,
                Description = description,
                Type = type,
                Severity = severity,
                Status = AnnouncementStatus.Pending,
                Location = location,
                ReportedAt = DateTime.UtcNow
            };

            announcement.SetCreatedInfo(createdBy);
            announcement.AddDomainEvent(new AnnouncementCreatedEvent(
                announcement.Id, tenantId, title, severity));

            return announcement;
        }

        public void Update(
            string title,
            string description,
            AnnouncementType type,
            Severity severity,
            string location,
            Guid updatedBy)
        {
            Title = title;
            Description = description;
            Type = type;
            Severity = severity;
            Location = location;

            SetUpdatedInfo(updatedBy);
        }

        public void AddImage(string imageUrl, string description = null, Guid? uploadedBy = null)
        {
            if (_images.Count >= 5)
                throw new BusinessRuleValidationException("No se pueden agregar más de 5 imágenes por anuncio");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new DomainException("La URL de la imagen es requerida");

            var sortOrder = _images.Count;
            _images.Add(AnnouncementImage.Create(Id, imageUrl, description, sortOrder, uploadedBy ?? CreatedBy));
        }

        public void RemoveImage(Guid imageId)
        {
            var image = _images.FirstOrDefault(i => i.Id == imageId);
            if (image == null)
                throw new EntityNotFoundException("Imagen no encontrada");

            _images.Remove(image);

            // Reordenar las imágenes restantes
            var index = 0;
            foreach (var img in _images.OrderBy(i => i.SortOrder))
            {
                img.UpdateSortOrder(index++);
            }
        }

        public CorrectiveAction AddCorrectiveAction(
            string description,
            Guid responsibleUserId,
            DateTime dueDate,
            Guid createdBy)
        {
            var action = CorrectiveAction.Create(
                Id, description, responsibleUserId, dueDate, createdBy);

            _correctiveActions.Add(action);
            UpdateStatus();

            return action;
        }

        public void CompleteCorrectiveAction(Guid actionId, Guid completedBy)
        {
            var action = _correctiveActions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
                throw new EntityNotFoundException("Acción correctiva no encontrada");

            action.Complete(completedBy);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (!_correctiveActions.Any())
            {
                Status = AnnouncementStatus.Pending;
            }
            else if (_correctiveActions.All(a => a.Status == ActionStatus.Completed))
            {
                Status = AnnouncementStatus.Completed;
                CompletedAt = DateTime.UtcNow;
            }
            else if (_correctiveActions.Any(a => a.Status == ActionStatus.InProgress))
            {
                Status = AnnouncementStatus.InProgress;
            }
            else
            {
                Status = AnnouncementStatus.InProgress;
            }
        }

        public void Complete(Guid completedBy)
        {
            if (Status == AnnouncementStatus.Completed)
                throw new BusinessRuleValidationException("El anuncio ya está completado");

            if (_correctiveActions.Any(a => a.Status != ActionStatus.Completed))
                throw new BusinessRuleValidationException("Todas las acciones correctivas deben estar completadas");

            Status = AnnouncementStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            SetUpdatedInfo(completedBy);
        }

        public decimal GetCompletionPercentage()
        {
            if (!_correctiveActions.Any()) return 0;

            var completed = _correctiveActions.Count(a => a.Status == ActionStatus.Completed);
            return (completed * 100m) / _correctiveActions.Count;
        }
    }
}