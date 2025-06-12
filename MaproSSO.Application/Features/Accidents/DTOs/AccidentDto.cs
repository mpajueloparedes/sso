namespace MaproSSO.Application.Features.Accidents.DTOs;

public class AccidentDto
{
    public Guid AccidentId { get; set; }
    public Guid TenantId { get; set; }
    public string AccidentCode { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public DateTime ReportedAt { get; set; }
    public string Shift { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImmediateCauses { get; set; }
    public string? RootCauses { get; set; }
    public string? WitnessNames { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? InvestigationStartDate { get; set; }
    public DateTime? InvestigationEndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<AccidentPersonDto> People { get; set; } = new();
    public List<AccidentImageDto> Images { get; set; } = new();
}

public class AccidentPersonDto
{
    public Guid PersonId { get; set; }
    public Guid AccidentId { get; set; }
    public string PersonType { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Position { get; set; }
    public string? Company { get; set; }
    public Guid? AreaId { get; set; }
    public string? AreaName { get; set; }
    public string? InjuryType { get; set; }
    public string? InjuryDescription { get; set; }
    public bool MedicalAttention { get; set; }
    public int? LostWorkDays { get; set; }
}

public class AccidentImageDto
{
    public Guid ImageId { get; set; }
    public Guid AccidentId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }
}