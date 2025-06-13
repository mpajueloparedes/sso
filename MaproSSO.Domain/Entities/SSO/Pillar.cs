//using MaproSSO.Domain.Common;
//using MaproSSO.Domain.Exceptions;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class Pillar : BaseAuditableEntity
//    {
//        public Guid TenantId { get; private set; }
//        public string PillarName { get; private set; }
//        public string PillarCode { get; private set; }
//        public string Description { get; private set; }
//        public string Icon { get; private set; }
//        public string Color { get; private set; }
//        public int SortOrder { get; private set; }
//        public bool IsActive { get; private set; }

//        private Pillar() { }

//        public static Pillar Create(
//            Guid tenantId,
//            string pillarName,
//            string pillarCode,
//            string description = null,
//            string icon = null,
//            string color = null,
//            int sortOrder = 0,
//            Guid? createdBy = null)
//        {
//            if (string.IsNullOrWhiteSpace(pillarName))
//                throw new DomainException("El nombre del pilar es requerido");

//            if (string.IsNullOrWhiteSpace(pillarCode))
//                throw new DomainException("El código del pilar es requerido");

//            var pillar = new Pillar
//            {
//                TenantId = tenantId,
//                PillarName = pillarName,
//                PillarCode = pillarCode.ToUpperInvariant(),
//                Description = description,
//                Icon = icon,
//                Color = color,
//                SortOrder = sortOrder,
//                IsActive = true
//            };

//            if (createdBy.HasValue)
//                pillar.SetCreatedInfo(createdBy.Value);

//            return pillar;
//        }

//        public void Update(
//            string pillarName,
//            string description,
//            string icon,
//            string color,
//            int sortOrder,
//            Guid updatedBy)
//        {
//            PillarName = pillarName;
//            Description = description;
//            Icon = icon;
//            Color = color;
//            SortOrder = sortOrder;

//            SetUpdatedInfo(updatedBy);
//        }
//    }
//}