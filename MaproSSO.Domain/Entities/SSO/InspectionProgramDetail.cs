//using MaproSSO.Domain.Common;
//using MaproSSO.Domain.Exceptions;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class InspectionProgramDetail : BaseEntity
//    {
//        public Guid ProgramId { get; private set; }
//        public Guid AreaId { get; private set; }
//        public string Frequency { get; private set; } // Daily, Weekly, Monthly, Quarterly, Annual
//        public DateTime StartDate { get; private set; }
//        public DateTime? EndDate { get; private set; }
//        public bool IsActive { get; private set; }

//        private InspectionProgramDetail() { }

//        public static InspectionProgramDetail Create(
//            Guid programId,
//            Guid areaId,
//            string frequency,
//            DateTime startDate,
//            DateTime? endDate = null)
//        {
//            var validFrequencies = new[] { "Daily", "Weekly", "Monthly", "Quarterly", "Annual" };
//            if (!validFrequencies.Contains(frequency))
//                throw new BusinessRuleValidationException("Frecuencia inválida");

//            if (endDate.HasValue && endDate.Value <= startDate)
//                throw new BusinessRuleValidationException("La fecha de fin debe ser posterior a la fecha de inicio");

//            return new InspectionProgramDetail
//            {
//                ProgramId = programId,
//                AreaId = areaId,
//                Frequency = frequency,
//                StartDate = startDate,
//                EndDate = endDate,
//                IsActive = true
//            };
//        }

//        public void Deactivate()
//        {
//            IsActive = false;
//            EndDate = DateTime.UtcNow;
//        }

//        public DateTime? GetNextInspectionDate(DateTime? lastInspectionDate = null)
//        {
//            if (!IsActive || (EndDate.HasValue && EndDate.Value < DateTime.UtcNow))
//                return null;

//            var baseDate = lastInspectionDate ?? StartDate;

//            return Frequency switch
//            {
//                "Daily" => baseDate.AddDays(1),
//                "Weekly" => baseDate.AddDays(7),
//                "Monthly" => baseDate.AddMonths(1),
//                "Quarterly" => baseDate.AddMonths(3),
//                "Annual" => baseDate.AddYears(1),
//                _ => null
//            };
//        }
//    }
//}