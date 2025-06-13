using MediatR;
using MaproSSO.Application.Features.Pillars.DTOs;

namespace MaproSSO.Application.Features.Pillars.Queries;

public record GetPillarsQuery : IRequest<List<PillarDto>>
{
    public bool? IsActive { get; init; }
    public bool IncludeStatistics { get; init; } = false;
}

public record GetPillarByIdQuery : IRequest<PillarDto?>
{
    public Guid PillarId { get; init; }
    public bool IncludeFolders { get; init; } = false;
}

public record GetFoldersByPillarQuery : IRequest<List<DocumentFolderDto>>
{
    public Guid PillarId { get; init; }
    public Guid? AreaId { get; init; }
    public bool IncludeDocuments { get; init; } = false;
}

public record GetFolderByIdQuery : IRequest<DocumentFolderDto?>
{
    public Guid FolderId { get; init; }
    public bool IncludeDocuments { get; init; } = true;
    public bool IncludeSubFolders { get; init; } = true;
}

public record GetFolderTreeQuery : IRequest<List<FolderTreeDto>>
{
    public Guid PillarId { get; init; }
    public Guid? AreaId { get; init; }
}

public record GetDocumentsByFolderQuery : IRequest<List<DocumentDto>>
{
    public Guid FolderId { get; init; }
    public bool IncludeDeleted { get; init; } = false;
    public bool CurrentVersionOnly { get; init; } = true;
    public string? SearchTerm { get; init; }
    public string? FileExtension { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetDocumentByIdQuery : IRequest<DocumentDto?>
{
    public Guid DocumentId { get; init; }
    public bool IncludeVersions { get; init; } = false;
}

public record SearchDocumentsQuery : IRequest<List<DocumentDto>>
{
    public string SearchTerm { get; init; } = string.Empty;
    public Guid? PillarId { get; init; }
    public Guid? AreaId { get; init; }
    public string? FileExtension { get; init; }
    public string? Tags { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record GetDocumentVersionsQuery : IRequest<List<DocumentVersionDto>>
{
    public Guid DocumentId { get; init; }
}

public record GetPillarStatisticsQuery : IRequest<PillarStatisticsDto>
{
    public Guid? PillarId { get; init; }
    public Guid? AreaId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}

public class PillarStatisticsDto
{
    public int TotalPillars { get; set; }
    public int TotalFolders { get; set; }
    public int TotalDocuments { get; set; }
    public int CurrentVersionDocuments { get; set; }
    public int DeletedDocuments { get; set; }
    public long TotalStorageBytes { get; set; }
    public string TotalStorageFormatted { get; set; } = string.Empty;
    public Dictionary<string, int> DocumentsByPillar { get; set; } = new();
    public Dictionary<string, int> DocumentsByExtension { get; set; } = new();
    public Dictionary<string, long> StorageByPillar { get; set; } = new();
    public Dictionary<string, int> DocumentsByMonth { get; set; } = new();
    public List<TopDocumentDto> MostAccessedDocuments { get; set; } = new();
    public List<TopDocumentDto> RecentDocuments { get; set; } = new();
}

public class TopDocumentDto
{
    public Guid DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string PillarName { get; set; } = string.Empty;
    public string FolderPath { get; set; } = string.Empty;
    public DateTime LastAccessed { get; set; }
    public int AccessCount { get; set; }
    public long FileSizeBytes { get; set; }
    public string FileSizeFormatted { get; set; } = string.Empty;
}