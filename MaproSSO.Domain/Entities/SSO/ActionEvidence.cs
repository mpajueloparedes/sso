using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class ActionEvidence : BaseEntity
    {
        public Guid ActionId { get; private set; }
        public string Description { get; private set; }
        public string EvidenceUrl { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public Guid UploadedBy { get; private set; }

        private ActionEvidence() { }

        public static ActionEvidence Create(
            Guid actionId,
            string description,
            string evidenceUrl,
            Guid uploadedBy)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("La descripción es requerida");

            if (string.IsNullOrWhiteSpace(evidenceUrl))
                throw new DomainException("La URL de evidencia es requerida");

            return new ActionEvidence
            {
                ActionId = actionId,
                Description = description,
                EvidenceUrl = evidenceUrl,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = uploadedBy
            };
        }
    }
}