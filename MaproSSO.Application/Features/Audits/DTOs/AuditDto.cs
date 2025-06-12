namespace MaproSSO.Application.Features.Audits.DTOs;

public class AuditDto
{
    public Guid AuditId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string AuditCode { get; set; } = string.Empty;
    public string AuditType { get; set; } = string.Empty;
    public Guid AuditorUserId { get; set; }
    public string AuditorName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ExecutedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal? TotalScore { get; set; }
    public decimal? MaxScore { get; set; }
    public decimal? CompliancePercentage { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AuditEvaluationDto> Evaluations { get; set; } = new();
}

public class AuditProgramDto
{
    public Guid ProgramId { get; set; }
    public Guid TenantId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Standard { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuditCategoryDto
{
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string CategoryCode { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public List<AuditCriteriaDto> Criteria { get; set; } = new();
}

public class AuditCriteriaDto
{
    public Guid CriteriaId { get; set; }
    public Guid CategoryId { get; set; }
    public string CriteriaCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MaxScore { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
}

public class AuditEvaluationDto
{
    public Guid EvaluationId { get; set; }
    public Guid AuditId { get; set; }
    public Guid CriteriaId { get; set; }
    public string CriteriaDescription { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public string? Observations { get; set; }
    public bool EvidenceRequired { get; set; }
    public DateTime EvaluatedAt { get; set; }
    public Guid EvaluatedBy { get; set; }
    public string EvaluatedByName { get; set; } = string.Empty;
    public List<AuditEvidenceDto> Evidences { get; set; } = new();
}

public class AuditEvidenceDto
{
    public Guid EvidenceId { get; set; }
    public Guid EvaluationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string EvidenceUrl { get; set; } = string.Empty;
    public Guid UploadedBy { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

