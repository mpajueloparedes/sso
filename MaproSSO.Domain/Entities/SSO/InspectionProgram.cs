using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class InspectionProgram : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<InspectionProgramDetail> _details = new();

        public Guid TenantId { get; private set; }
        public string ProgramName { get; private set; }
        public string ProgramType { get; private set; }
        public string Description { get; private set; }
        public int Year { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<InspectionProgramDetail> Details => _details.AsReadOnly();

        private InspectionProgram() { }

        public static InspectionProgram Create(
            Guid tenantId,
            string programName,
            string programType,
            int year,
            string description = null,
            Guid? createdBy = null)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new DomainException("El nombre del programa es requerido");

            if (string.IsNullOrWhiteSpace(programType))
                throw new DomainException("El tipo de programa es requerido");

            if (year < 2020)
                throw new BusinessRuleValidationException("El año del programa no es válido");

            var program = new InspectionProgram
            {
                TenantId = tenantId,
                ProgramName = programName,
                ProgramType = programType,
                Description = description,
                Year = year,
                IsActive = true
            };

            if (createdBy.HasValue)
                program.SetCreatedInfo(createdBy.Value);

            return program;
        }

        public void AddDetail(Guid areaId, string frequency, DateTime startDate, DateTime? endDate = null)
        {
            if (_details.Any(d => d.AreaId == areaId && d.IsActive))
                throw new BusinessRuleValidationException("Ya existe una configuración activa para esta área");

            var detail = InspectionProgramDetail.Create(Id, areaId, frequency, startDate, endDate);
            _details.Add(detail);
        }

        public void RemoveDetail(Guid detailId)
        {
            var detail = _details.FirstOrDefault(d => d.Id == detailId);
            if (detail != null)
            {
                detail.Deactivate();
            }
        }

        public void Deactivate(Guid userId)
        {
            IsActive = false;
            SetUpdatedInfo(userId);

            foreach (var detail in _details.Where(d => d.IsActive))
            {
                detail.Deactivate();
            }
        }
    }
}