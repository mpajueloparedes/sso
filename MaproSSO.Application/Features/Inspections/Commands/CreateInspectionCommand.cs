using MediatR;
using MaproSSO.Application.Features.Inspections.DTOs;

namespace MaproSSO.Application.Features.Inspections.Commands;

public record CreateInspectionCommand : IRequest<InspectionDto>
{
    public Guid ProgramId { get; init; }
    public Guid AreaId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid InspectorUserId { get; init; }
    public DateTime ScheduledDate { get; init; }
}

public record UpdateInspectionCommand : IRequest<InspectionDto>
{
    public Guid InspectionId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Guid InspectorUserId { get; init; }
    public DateTime ScheduledDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public int CompletionPercentage { get; init; }
}

public record DeleteInspectionCommand : IRequest<bool>
{
    public Guid InspectionId { get; init; }
}

public record CompleteInspectionCommand : IRequest<InspectionDto>
{
    public Guid InspectionId { get; init; }
    public string? DocumentUrl { get; init; }
}

public record CreateObservationCommand : IRequest<InspectionObservationDto>
{
    public Guid InspectionId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Severity { get; init; } = string.Empty;
    public Guid ResponsibleUserId { get; init; }
    public DateTime DueDate { get; init; }
    public List<string> ImageUrls { get; init; } = new();
}
