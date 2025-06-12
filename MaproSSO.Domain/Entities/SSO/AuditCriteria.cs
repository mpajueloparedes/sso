using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class AuditCriteria : BaseEntity
    {
        public Guid CategoryId { get; private set; }
        public string CriteriaCode { get; private set; }
        public string Description { get; private set; }
        public decimal MaxScore { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        private AuditCriteria() { }
    }
}