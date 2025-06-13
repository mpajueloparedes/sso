using MediatR;
using MaproSSO.Application.Features.Pillars.DTOs;

namespace MaproSSO.Application.Features.Pillars.Commands;

public record CreatePillarCommand : IRequest<PillarDto>
{
    public string PillarName { get; init; } = string.Empty;
    public string PillarCode { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int SortOrder { get; init; } = 0;
}

public record UpdatePillarCommand : IRequest<PillarDto>
{
    public Guid PillarId { get; init; }
    public string PillarName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Icon { get; init; }
    public string? Color { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}

public record DeletePillarCommand : IRequest<bool>
{
    public Guid PillarId { get; init; }
}

public record CreateFolderCommand : IRequest<DocumentFolderDto>
{
    public Guid PillarId { get; init; }
    public Guid AreaId { get; init; }
    public string FolderName { get; init; } = string.Empty;
    public Guid? ParentFolderId { get; init; }
}

public record UpdateFolderCommand : IRequest<DocumentFolderDto>
{
    public Guid FolderId { get; init; }
    public string FolderName { get; init; } = string.Empty;
    public Guid? ParentFolderId { get; init; }
}

public record DeleteFolderCommand : IRequest<bool>
{
    public Guid FolderId { get; init; }
    public bool DeleteContents { get; init; } = false;
}

public record UploadDocumentCommand : IRequest<DocumentDto>
{
    public Guid FolderId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string StorageUrl { get; init; } = string.Empty;
    public string? Tags { get; init; }
    public bool ReplaceExisting { get; init; } = false;
}

public record UpdateDocumentCommand : IRequest<DocumentDto>
{
    public Guid DocumentId { get; init; }
    public string? FileName { get; init; }
    public string? Tags { get; init; }
    public Guid? FolderId { get; init; }
}

public record CreateDocumentVersionCommand : IRequest<DocumentDto>
{
    public Guid ParentDocumentId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
    public string StorageUrl { get; init; } = string.Empty;
    public string? Comments { get; init; }
}

public record DeleteDocumentCommand : IRequest<bool>
{
    public Guid DocumentId { get; init; }
    public bool PermanentDelete { get; init; } = false;
}

public record RestoreDocumentCommand : IRequest<DocumentDto>
{
    public Guid DocumentId { get; init; }
}