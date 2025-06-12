using MediatR;
using MaproSSO.Application.Features.Accidents.DTOs;

namespace MaproSSO.Application.Features.Accidents.Queries;

public record GetAccidentByIdQuery : IRequest<AccidentDto?>
{
    public Guid AccidentId { get; init; }
}

public record GetAccidentsByDateRangeQuery : IRequest<List<AccidentDto>>
{
    public DateTime FromDate { get; init; }
    public DateTime ToDate { get; init; }
    public string? Type { get; init; }
    public string? Status { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public record GetAccidentStatisticsQuery : IRequest<AccidentStatisticsDto>
{
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public Guid? AreaId { get; init; }
}

public class AccidentStatisticsDto
{
    public int TotalAccidents { get; set; }
    public int TotalIncidents { get; set; }
    public int TotalNearMisses { get; set; }
    public int FatalAccidents { get; set; }
    public int SeriousAccidents { get; set; }
    public int ModerateAccidents { get; set; }
    public int MinorAccidents { get; set; }
    public int PeopleAffected { get; set; }
    public int TotalLostWorkDays { get; set; }
    public int AccidentsUnderInvestigation { get; set; }
    public Dictionary<string, int> AccidentsByType { get; set; } = new();
    public Dictionary<string, int> AccidentsBySeverity { get; set; } = new();
    public Dictionary<string, int> AccidentsByShift { get; set; } = new();
    public Dictionary<string, int> AccidentsByMonth { get; set; } = new();
}
