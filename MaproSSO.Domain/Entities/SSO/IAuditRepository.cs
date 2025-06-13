using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public interface IAuditRepository : IRepository<MaproSSO.Domain.Entities.Audits.Audit>
    {
        Task<MaproSSO.Domain.Entities.Audits.Audit> GetWithEvaluationsAsync(Guid auditId);
        Task<IEnumerable<MaproSSO.Domain.Entities.Audits.Audit>> GetByProgramAsync(Guid programId);
        Task<IEnumerable<MaproSSO.Domain.Entities.Audits.Audit>> GetByAreaAsync(Guid areaId);
        Task<IEnumerable<MaproSSO.Domain.Entities.Audits.Audit>> GetPendingAuditsAsync(Guid tenantId);
        Task<IEnumerable<MaproSSO.Domain.Entities.Audits.AuditEvaluation>> GetEvaluationsByCriteriaAsync(Guid criteriaId);
        Task<Dictionary<Guid, decimal>> GetAverageScoresByCategoryAsync(Guid auditId);
        Task<int> GetCompletedEvaluationsCountAsync(Guid auditId);
        Task<bool> HasEvaluationsAsync(Guid auditId);
    }
}