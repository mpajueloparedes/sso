//using MaproSSO.Domain.Common;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class ObservationEvidence : BaseEntity
//    {
//        public Guid ObservationId { get; private set; }
//        public string Description { get; private set; }
//        public string EvidenceUrl { get; private set; }
//        public DateTime UploadedAt { get; private set; }
//        public Guid UploadedBy { get; private set; }

//        private ObservationEvidence() { }

//        public static ObservationEvidence Create(
//            Guid observationId,
//            string description,
//            string evidenceUrl,
//            Guid uploadedBy)
//        {
//            return new ObservationEvidence
//            {
//                ObservationId = observationId,
//                Description = description,
//                EvidenceUrl = evidenceUrl,
//                UploadedAt = DateTime.UtcNow,
//                UploadedBy = uploadedBy
//            };
//        }
//    }
//}