using MediatR;
using MaproSSO.Application.Features.Accidents.DTOs;

namespace MaproSSO.Application.Features.Accidents.Commands;

public record CreateAccidentCommand : IRequest<AccidentDto>
{
    public string Type { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; }
    public string Shift { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? WitnessNames { get; init; }
    public List<CreateAccidentPersonDto> People { get; init; } = new();
    public List<string> ImageUrls { get; init; } = new();
}

public record UpdateAccidentCommand : IRequest<AccidentDto>
{
    public Guid AccidentId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public DateTime OccurredAt { get; init; }
    public string Shift { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string? ImmediateCauses { get; init; }
    public string? RootCauses { get; init; }
    public string? WitnessNames { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record StartInvestigationCommand : IRequest<AccidentDto>
{
    public Guid AccidentId { get; init; }
}

public record CloseInvestigationCommand : IRequest<AccidentDto>
{
    public Guid AccidentId { get; init; }
    public string ImmediateCauses { get; init; } = string.Empty;
    public string RootCauses { get; init; } = string.Empty;
}

public class CreateAccidentPersonDto
{
    public string PersonType { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? DocumentNumber { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public string? Position { get; set; }
    public string? Company { get; set; }
    public Guid? AreaId { get; set; }
    public string? InjuryType { get; set; }
    public string? InjuryDescription { get; set; }
    public bool MedicalAttention { get; set; }
    public int? LostWorkDays { get; set; }
}