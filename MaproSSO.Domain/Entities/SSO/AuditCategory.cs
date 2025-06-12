using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class AuditCategory : BaseEntity
    {
        private readonly List<AuditCriteria> _criteria = new();

        public string CategoryName { get; private set; }
        public string CategoryCode { get; private set; }
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<AuditCriteria> Criteria => _criteria.AsReadOnly();

        private AuditCategory() { }

        public static AuditCategory Create(
            string categoryName,
            string categoryCode,
            int sortOrder,
            string description = null)
        {
            return new AuditCategory
            {
                CategoryName = categoryName,
                CategoryCode = categoryCode.ToUpperInvariant(),
                Description = description,
                SortOrder = sortOrder,
                IsActive = true
            };
        }

        public void AddCriteria(
            string criteriaCode,
            string description,
            decimal maxScore,
            int sortOrder)
        {
            if (_criteria.Any(c => c.CriteriaCode == criteriaCode))
                throw new BusinessRuleValidationException("Ya existe un criterio con este código");

            var criteria = AuditCriteria.Create(Id, criteriaCode, description, maxScore, sortOrder);
            _criteria.Add(criteria);
        }
    }
}