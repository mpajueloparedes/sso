using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Audits.Commands;
using MaproSSO.Application.Features.Audits.DTOs;
using MaproSSO.Domain.Entities.Audits;

namespace MaproSSO.Application.Features.Audits.Handlers;

public class CreateAuditHandler : IRequestHandler<CreateAuditCommand, AuditDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateAuditHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AuditDto> Handle(CreateAuditCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Generate audit code
        var year = request.ScheduledDate.Year;
        var count = await _context.Audits
            .Where(a => a.TenantId == tenantId && a.ScheduledDate.Year == year)
            .CountAsync(cancellationToken);

        var auditCode = $"AUD-{year}-{(count + 1):D4}";

        var audit = new Audit
        {
            AuditId = Guid.NewGuid(),
            TenantId = tenantId,
            ProgramId = request.ProgramId,
            AreaId = request.AreaId,
            AuditCode = auditCode,
            AuditType = request.AuditType,
            AuditorUserId = request.AuditorUserId,
            ScheduledDate = request.ScheduledDate,
            Status = "Scheduled",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Audits.Add(audit);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Audits
            .Include(a => a.Program)
            .Include(a => a.Area)
            .Include(a => a.Auditor)
            .FirstAsync(a => a.AuditId == audit.AuditId, cancellationToken);

        return _mapper.Map<AuditDto>(result);
    }
}

public class EvaluateAuditCriteriaHandler : IRequestHandler<EvaluateAuditCriteriaCommand, AuditEvaluationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public EvaluateAuditCriteriaHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AuditEvaluationDto> Handle(EvaluateAuditCriteriaCommand request, CancellationToken cancellationToken)
    {
        // Check if evaluation already exists
        var existingEvaluation = await _context.AuditEvaluations
            .FirstOrDefaultAsync(e => e.AuditId == request.AuditId && e.CriteriaId == request.CriteriaId, cancellationToken);

        if (existingEvaluation != null)
        {
            // Update existing evaluation
            existingEvaluation.Score = request.Score;
            existingEvaluation.Observations = request.Observations;
            existingEvaluation.EvidenceRequired = request.EvidenceRequired;
            existingEvaluation.EvaluatedAt = DateTime.UtcNow;
            existingEvaluation.EvaluatedBy = _currentUserService.UserId;
            existingEvaluation.UpdatedAt = DateTime.UtcNow;
            existingEvaluation.UpdatedBy = _currentUserService.UserId;
        }
        else
        {
            // Create new evaluation
            existingEvaluation = new AuditEvaluation
            {
                EvaluationId = Guid.NewGuid(),
                AuditId = request.AuditId,
                CriteriaId = request.CriteriaId,
                Score = request.Score,
                Observations = request.Observations,
                EvidenceRequired = request.EvidenceRequired,
                EvaluatedAt = DateTime.UtcNow,
                EvaluatedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId
            };

            _context.AuditEvaluations.Add(existingEvaluation);
        }

        // Add evidences if provided
        if (request.EvidenceUrls.Any())
        {
            var evidences = request.EvidenceUrls.Select(url => new AuditEvidence
            {
                EvidenceId = Guid.NewGuid(),
                EvaluationId = existingEvaluation.EvaluationId,
                Description = "Evidencia de auditoría",
                EvidenceUrl = url,
                UploadedBy = _currentUserService.UserId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _currentUserService.UserId
            });

            _context.AuditEvidences.AddRange(evidences);
        }

        // Update audit scores
        await UpdateAuditScores(request.AuditId, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.AuditEvaluations
            .Include(e => e.Criteria)
            .Include(e => e.EvaluatedByUser)
            .Include(e => e.Evidences)
                .ThenInclude(ev => ev.UploadedByUser)
            .FirstAsync(e => e.EvaluationId == existingEvaluation.EvaluationId, cancellationToken);

        return _mapper.Map<AuditEvaluationDto>(result);
    }

    private async Task UpdateAuditScores(Guid auditId, CancellationToken cancellationToken)
    {
        var audit = await _context.Audits.FirstAsync(a => a.AuditId == auditId, cancellationToken);

        var evaluations = await _context.AuditEvaluations
            .Include(e => e.Criteria)
            .Where(e => e.AuditId == auditId)
            .ToListAsync(cancellationToken);

        audit.TotalScore = evaluations.Sum(e => e.Score);
        audit.MaxScore = evaluations.Sum(e => e.Criteria.MaxScore);
    }
}