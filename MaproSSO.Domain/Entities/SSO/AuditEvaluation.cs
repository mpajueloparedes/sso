//using System;
//using System.Collections.Generic;
//using System.Linq;
//using MaproSSO.Domain.Common;
//using MaproSSO.Domain.Exceptions;

//namespace MaproSSO.Domain.Entities.SSO
//{
//    public class AuditEvaluation : BaseEntity
//    {
//        private readonly List<AuditEvidence> _evidences = new();

//        public Guid AuditId { get; private set; }
//        public Guid CriteriaId { get; private set; }
//        public decimal Score { get; private set; }
//        public decimal MaxScore { get; private set; }
//        public string Observations { get; private set; }
//        public bool EvidenceRequired { get; private set; }
//        public DateTime EvaluatedAt { get; private set; }
//        public Guid EvaluatedBy { get; private set; }

//        public virtual Audit Audit { get; private set; }
//        public virtual AuditCriteria Criteria { get; private set; }
//        public IReadOnlyCollection<AuditEvidence> Evidences => _evidences.AsReadOnly();

//        // Propiedades calculadas
//        public decimal ScorePercentage => MaxScore > 0 ? (Score / MaxScore) * 100 : 0;
//        public bool IsComplete => !EvidenceRequired || _evidences.Any();
//        public bool NeedsImprovement => ScorePercentage < 70;

//        private AuditEvaluation() { }

//        public static AuditEvaluation Create(
//            Guid auditId,
//            Guid criteriaId,
//            decimal score,
//            decimal maxScore,
//            string observations = null,
//            bool evidenceRequired = false,
//            Guid? evaluatedBy = null)
//        {
//            if (score < 0)
//                throw new DomainException("El puntaje no puede ser negativo");

//            if (maxScore <= 0)
//                throw new DomainException("El puntaje máximo debe ser mayor a cero");

//            if (score > maxScore)
//                throw new BusinessRuleValidationException("El puntaje no puede ser mayor al puntaje máximo");

//            return new AuditEvaluation
//            {
//                AuditId = auditId,
//                CriteriaId = criteriaId,
//                Score = score,
//                MaxScore = maxScore,
//                Observations = observations,
//                EvidenceRequired = evidenceRequired,
//                EvaluatedAt = DateTime.UtcNow,
//                EvaluatedBy = evaluatedBy ?? Guid.Empty
//            };
//        }

//        public void UpdateScore(decimal newScore, string observations)
//        {
//            if (newScore < 0)
//                throw new DomainException("El puntaje no puede ser negativo");

//            if (newScore > MaxScore)
//                throw new BusinessRuleValidationException($"El puntaje no puede ser mayor a {MaxScore}");

//            Score = newScore;
//            Observations = observations;
//            EvaluatedAt = DateTime.UtcNow;
//        }

//        public void AddEvidence(string description, string evidenceUrl, Guid uploadedBy)
//        {
//            if (string.IsNullOrWhiteSpace(description))
//                throw new DomainException("La descripción de la evidencia es requerida");

//            if (string.IsNullOrWhiteSpace(evidenceUrl))
//                throw new DomainException("La URL de la evidencia es requerida");

//            var evidence = AuditEvidence.Create(Id, description, evidenceUrl, uploadedBy);
//            _evidences.Add(evidence);
//        }

//        public void RemoveEvidence(Guid evidenceId)
//        {
//            var evidence = _evidences.FirstOrDefault(e => e.Id == evidenceId);
//            if (evidence == null)
//                throw new EntityNotFoundException("Evidencia no encontrada");

//            _evidences.Remove(evidence);
//        }

//        public void MarkEvidenceAsRequired()
//        {
//            EvidenceRequired = true;
//        }

//        public void MarkEvidenceAsOptional()
//        {
//            EvidenceRequired = false;
//        }

//        public string GetComplianceLevel()
//        {
//            return ScorePercentage switch
//            {
//                >= 90 => "Excelente",
//                >= 80 => "Bueno",
//                >= 70 => "Aceptable",
//                >= 60 => "Requiere Mejora",
//                _ => "Crítico"
//            };
//        }

//        public string GetComplianceColor()
//        {
//            return ScorePercentage switch
//            {
//                >= 90 => "#28a745", // Verde
//                >= 80 => "#17a2b8", // Azul
//                >= 70 => "#ffc107", // Amarillo
//                >= 60 => "#fd7e14", // Naranja
//                _ => "#dc3545"       // Rojo
//            };
//        }
//    }
//}