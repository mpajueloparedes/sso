//using MaproSSO.Domain.Common;
//using MaproSSO.Domain.Exceptions;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class AuditEvidence : BaseEntity
//    {
//        public Guid EvaluationId { get; private set; }
//        public string Description { get; private set; }
//        public string EvidenceUrl { get; private set; }
//        public string FileType { get; private set; }
//        public long FileSizeBytes { get; private set; }
//        public DateTime UploadedAt { get; private set; }
//        public Guid UploadedBy { get; private set; }

//        public virtual AuditEvaluation Evaluation { get; private set; }

//        private AuditEvidence() { }

//        public static AuditEvidence Create(
//            Guid evaluationId,
//            string description,
//            string evidenceUrl,
//            Guid uploadedBy,
//            string fileType = null,
//            long fileSizeBytes = 0)
//        {
//            if (string.IsNullOrWhiteSpace(description))
//                throw new DomainException("La descripción es requerida");

//            if (string.IsNullOrWhiteSpace(evidenceUrl))
//                throw new DomainException("La URL de evidencia es requerida");

//            return new AuditEvidence
//            {
//                EvaluationId = evaluationId,
//                Description = description,
//                EvidenceUrl = evidenceUrl,
//                FileType = fileType ?? GetFileTypeFromUrl(evidenceUrl),
//                FileSizeBytes = fileSizeBytes,
//                UploadedAt = DateTime.UtcNow,
//                UploadedBy = uploadedBy
//            };
//        }

//        private static string GetFileTypeFromUrl(string url)
//        {
//            var extension = System.IO.Path.GetExtension(url)?.ToLowerInvariant();
//            return extension switch
//            {
//                ".pdf" => "application/pdf",
//                ".doc" => "application/msword",
//                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
//                ".xls" => "application/vnd.ms-excel",
//                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                ".jpg" or ".jpeg" => "image/jpeg",
//                ".png" => "image/png",
//                ".gif" => "image/gif",
//                _ => "application/octet-stream"
//            };
//        }

//        public void UpdateDescription(string newDescription)
//        {
//            if (string.IsNullOrWhiteSpace(newDescription))
//                throw new DomainException("La descripción es requerida");

//            Description = newDescription;
//        }
//    }
//}