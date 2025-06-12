namespace MaproSSO.Application.Features.Inspections.DTOs;

public class InspectionDto
{
    public Guid InspectionId { get; set; }
    public Guid TenantId { get; set; }
    public Guid ProgramId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string InspectionCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InspectorUserId { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ExecutedDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CompletionPercentage { get; set; }
    public string? DocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<InspectionObservationDto> Observations { get; set; } = new();
}

public class InspectionProgramDto
{
    public Guid ProgramId { get; set; }
    public Guid TenantId { get; set; }
    public string ProgramName { get; set; } = string.Empty;
    public string ProgramType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Year { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<InspectionProgramDetailDto> ProgramDetails { get; set; } = new();
}

public class InspectionProgramDetailDto
{
    public Guid DetailId { get; set; }
    public Guid ProgramId { get; set; }
    public Guid AreaId { get; set; }
    public string AreaName { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}

public class InspectionObservationDto
{
    public Guid ObservationId { get; set; }
    public Guid InspectionId { get; set; }
    public string ObservationCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public Guid ResponsibleUserId { get; set; }
    public string ResponsibleUserName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }
    public List<ObservationImageDto> Images { get; set; } = new();
    public List<ObservationEvidenceDto> Evidences { get; set; } = new();
}

public class ObservationImageDto
{
    public Guid ImageId { get; set; }
    public Guid ObservationId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}

public class ObservationEvidenceDto
{
    public Guid EvidenceId { get; set; }
    public Guid ObservationId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? EvidenceUrl { get; set; }
    public Guid UploadedBy { get; set; }
    public string UploadedByName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}