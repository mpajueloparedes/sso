using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

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
        public string EvaluationGuideline { get; private set; }
        public bool RequiresEvidence { get; private set; }

        public virtual AuditCategory Category { get; private set; }

        private AuditCriteria() { }

        public static AuditCriteria Create(
            Guid categoryId,
            string criteriaCode,
            string description,
            decimal maxScore,
            int sortOrder,
            string evaluationGuideline = null,
            bool requiresEvidence = false)
        {
            if (string.IsNullOrWhiteSpace(criteriaCode))
                throw new DomainException("El código del criterio es requerido");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("La descripción es requerida");

            if (maxScore <= 0)
                throw new DomainException("El puntaje máximo debe ser mayor a cero");

            return new AuditCriteria
            {
                CategoryId = categoryId,
                CriteriaCode = criteriaCode.ToUpperInvariant(),
                Description = description,
                MaxScore = maxScore,
                SortOrder = sortOrder,
                EvaluationGuideline = evaluationGuideline,
                RequiresEvidence = requiresEvidence,
                IsActive = true
            };
        }

        public void Update(
            string description,
            decimal maxScore,
            string evaluationGuideline,
            bool requiresEvidence)
        {
            Description = description;
            MaxScore = maxScore;
            EvaluationGuideline = evaluationGuideline;
            RequiresEvidence = requiresEvidence;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public string GetScoreRange()
        {
            return $"0 - {MaxScore:N1}";
        }
    }
}