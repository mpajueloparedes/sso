using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public interface IAuditRepository : IRepository<Audit>
    {
        Task<Audit> GetWithEvaluationsAsync(Guid auditId);
        Task<IEnumerable<Audit>> GetByProgramAsync(Guid programId);
        Task<IEnumerable<Audit>> GetByAreaAsync(Guid areaId);
        Task<IEnumerable<Audit>> GetPendingAuditsAsync(Guid tenantId);
        Task<IEnumerable<AuditEvaluation>> GetEvaluationsByCriteriaAsync(Guid criteriaId);
        Task<Dictionary<Guid, decimal>> GetAverageScoresByCategoryAsync(Guid auditId);
        Task<int> GetCompletedEvaluationsCountAsync(Guid auditId);
        Task<bool> HasEvaluationsAsync(Guid auditId);
    }
}