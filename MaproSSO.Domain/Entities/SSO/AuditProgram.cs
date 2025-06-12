using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.SSO
{
    public class AuditProgram : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<Audit> _audits = new();

        public Guid TenantId { get; private set; }
        public string ProgramName { get; private set; }
        public int Year { get; private set; }
        public string Standard { get; private set; } // ISO 45001, OHSAS 18001, etc.
        public string Description { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Audit> Audits => _audits.AsReadOnly();

        private AuditProgram() { }

        public static AuditProgram Create(
            Guid tenantId,
            string programName,
            int year,
            string standard,
            DateTime startDate,
            DateTime endDate,
            string description = null,
            Guid? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new DomainException("El nombre del programa es requerido");

            if (string.IsNullOrWhiteSpace(standard))
                throw new DomainException("El estándar es requerido");

            if (endDate <= startDate)
                throw new BusinessRuleValidationException("La fecha de fin debe ser posterior a la fecha de inicio");

            var program = new AuditProgram
            {
                TenantId = tenantId,
                ProgramName = programName,
                Year = year,
                Standard = standard,
                Description = description,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = true
            };

            if (createdBy.HasValue)
                program.SetCreatedInfo(createdBy.Value);

            return program;
        }

        public void Update(
            string programName,
            string standard,
            string description,
            DateTime startDate,
            DateTime endDate,
            Guid updatedBy)
        {
            ProgramName = programName;
            Standard = standard;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;

            SetUpdatedInfo(updatedBy);
        }

        public void Deactivate(Guid userId)
        {
            IsActive = false;
            SetUpdatedInfo(userId);
        }
    }
}
