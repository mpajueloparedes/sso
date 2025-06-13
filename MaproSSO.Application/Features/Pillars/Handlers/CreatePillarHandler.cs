using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Pillars.Commands;
using MaproSSO.Application.Features.Pillars.DTOs;
using MaproSSO.Domain.Entities.Pillars;
using MaproSSO.Application.Features.Pillars.Queries;

namespace MaproSSO.Application.Features.Pillars.Handlers;

public class CreatePillarHandler : IRequestHandler<CreatePillarCommand, PillarDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreatePillarHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<PillarDto> Handle(CreatePillarCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Check if pillar code already exists
        var existingPillar = await _context.Pillars
            .FirstOrDefaultAsync(p => p.TenantId == tenantId && p.PillarCode == request.PillarCode, cancellationToken);

        if (existingPillar != null)
        {
            throw new InvalidOperationException("Pillar code already exists");
        }

        var pillar = new Pillar
        {
            PillarId = Guid.NewGuid(),
            TenantId = tenantId,
            PillarName = request.PillarName,
            PillarCode = request.PillarCode,
            Description = request.Description,
            Icon = request.Icon,
            Color = request.Color,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Pillars.Add(pillar);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PillarDto>(pillar);
    }
}

public class CreateFolderHandler : IRequestHandler<CreateFolderCommand, DocumentFolderDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateFolderHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DocumentFolderDto> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Build folder path
        var path = await BuildFolderPath(request.ParentFolderId, request.FolderName, cancellationToken);

        var folder = new DocumentFolder
        {
            FolderId = Guid.NewGuid(),
            TenantId = tenantId,
            PillarId = request.PillarId,
            AreaId = request.AreaId,
            FolderName = request.FolderName,
            ParentFolderId = request.ParentFolderId,
            Path = path,
            IsSystemFolder = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.DocumentFolders.Add(folder);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.DocumentFolders
            .Include(f => f.Pillar)
            .Include(f => f.Area)
            .Include(f => f.ParentFolder)
            .FirstAsync(f => f.FolderId == folder.FolderId, cancellationToken);

        return _mapper.Map<DocumentFolderDto>(result);
    }

    private async Task<string> BuildFolderPath(Guid? parentFolderId, string folderName, CancellationToken cancellationToken)
    {
        if (parentFolderId == null)
        {
            return folderName;
        }

        var parentFolder = await _context.DocumentFolders
            .FirstOrDefaultAsync(f => f.FolderId == parentFolderId, cancellationToken);

        if (parentFolder == null)
        {
            return folderName;
        }

        return $"{parentFolder.Path}/{folderName}";
    }
}

public class UploadDocumentHandler : IRequestHandler<UploadDocumentCommand, DocumentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UploadDocumentHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<DocumentDto> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Check if document with same name exists
        var existingDocument = await _context.Documents
            .FirstOrDefaultAsync(d => d.FolderId == request.FolderId &&
                                     d.FileName == request.FileName &&
                                     d.IsCurrentVersion &&
                                     d.DeletedAt == null, cancellationToken);

        if (existingDocument != null)
        {
            if (request.ReplaceExisting)
            {
                // Create new version
                return await CreateNewVersion(existingDocument, request, cancellationToken);
            }
            else
            {
                throw new InvalidOperationException("Document with same name already exists in this folder");
            }
        }

        // Create new document
        var document = new Document
        {
            DocumentId = Guid.NewGuid(),
            TenantId = tenantId,
            FolderId = request.FolderId,
            FileName = request.FileName,
            FileExtension = Path.GetExtension(request.FileName).ToLowerInvariant(),
            FileSizeBytes = request.FileSizeBytes,
            ContentType = request.ContentType,
            StorageUrl = request.StorageUrl,
            Version = 1,
            IsCurrentVersion = true,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Documents
            .Include(d => d.Folder)
                .ThenInclude(f => f.Pillar)
            .Include(d => d.Folder)
                .ThenInclude(f => f.Area)
            .Include(d => d.CreatedByUser)
            .FirstAsync(d => d.DocumentId == document.DocumentId, cancellationToken);

        return _mapper.Map<DocumentDto>(result);
    }

    private async Task<DocumentDto> CreateNewVersion(Document existingDocument, UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        // Mark current version as not current
        existingDocument.IsCurrentVersion = false;

        // Get next version number
        var maxVersion = await _context.Documents
            .Where(d => d.ParentDocumentId == existingDocument.DocumentId || d.DocumentId == existingDocument.DocumentId)
            .MaxAsync(d => d.Version, cancellationToken);

        var newVersion = new Document
        {
            DocumentId = Guid.NewGuid(),
            TenantId = existingDocument.TenantId,
            FolderId = request.FolderId,
            FileName = request.FileName,
            FileExtension = Path.GetExtension(request.FileName).ToLowerInvariant(),
            FileSizeBytes = request.FileSizeBytes,
            ContentType = request.ContentType,
            StorageUrl = request.StorageUrl,
            Version = maxVersion + 1,
            IsCurrentVersion = true,
            ParentDocumentId = existingDocument.ParentDocumentId ?? existingDocument.DocumentId,
            Tags = request.Tags,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Documents.Add(newVersion);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Documents
            .Include(d => d.Folder)
                .ThenInclude(f => f.Pillar)
            .Include(d => d.Folder)
                .ThenInclude(f => f.Area)
            .Include(d => d.CreatedByUser)
            .FirstAsync(d => d.DocumentId == newVersion.DocumentId, cancellationToken);

        return _mapper.Map<DocumentDto>(result);
    }
}

public class GetPillarsHandler : IRequestHandler<GetPillarsQuery, List<PillarDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetPillarsHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<PillarDto>> Handle(GetPillarsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        var query = _context.Pillars
            .Where(p => p.TenantId == tenantId);

        if (request.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == request.IsActive.Value);
        }

        var pillars = await query
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.PillarName)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<PillarDto>>(pillars);

        if (request.IncludeStatistics)
        {
            foreach (var pillar in result)
            {
                var stats = await GetPillarStatistics(pillar.PillarId, cancellationToken);
                pillar.TotalFolders = stats.TotalFolders;
                pillar.TotalDocuments = stats.TotalDocuments;
            }
        }

        return result;
    }

    private async Task<(int TotalFolders, int TotalDocuments)> GetPillarStatistics(Guid pillarId, CancellationToken cancellationToken)
    {
        var totalFolders = await _context.DocumentFolders
            .CountAsync(f => f.PillarId == pillarId, cancellationToken);

        var totalDocuments = await _context.Documents
            .Join(_context.DocumentFolders, d => d.FolderId, f => f.FolderId, (d, f) => new { d, f })
            .CountAsync(x => x.f.PillarId == pillarId && x.d.IsCurrentVersion && x.d.DeletedAt == null, cancellationToken);

        return (totalFolders, totalDocuments);
    }
}

public class GetFolderByIdHandler : IRequestHandler<GetFolderByIdQuery, DocumentFolderDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFolderByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DocumentFolderDto?> Handle(GetFolderByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.DocumentFolders
            .Include(f => f.Pillar)
            .Include(f => f.Area)
            .Include(f => f.ParentFolder)
            .AsQueryable();

        if (request.IncludeSubFolders)
        {
            query = query.Include(f => f.SubFolders);
        }

        if (request.IncludeDocuments)
        {
            query = query.Include(f => f.Documents.Where(d => d.IsCurrentVersion && d.DeletedAt == null))
                         .ThenInclude(d => d.CreatedByUser);
        }

        var folder = await query.FirstOrDefaultAsync(f => f.FolderId == request.FolderId, cancellationToken);

        return folder != null ? _mapper.Map<DocumentFolderDto>(folder) : null;
    }
}

public class SearchDocumentsHandler : IRequestHandler<SearchDocumentsQuery, List<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public SearchDocumentsHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<DocumentDto>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        var query = _context.Documents
            .Include(d => d.Folder)
                .ThenInclude(f => f.Pillar)
            .Include(d => d.Folder)
                .ThenInclude(f => f.Area)
            .Include(d => d.CreatedByUser)
            .Include(d => d.UpdatedByUser)
            .Where(d => d.TenantId == tenantId && d.IsCurrentVersion && d.DeletedAt == null);

        // Search term filter
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(d =>
                d.FileName.ToLower().Contains(searchTerm) ||
                (d.Tags != null && d.Tags.ToLower().Contains(searchTerm)));
        }

        // Pillar filter
        if (request.PillarId.HasValue)
        {
            query = query.Where(d => d.Folder.PillarId == request.PillarId.Value);
        }

        // Area filter
        if (request.AreaId.HasValue)
        {
            query = query.Where(d => d.Folder.AreaId == request.AreaId.Value);
        }

        // File extension filter
        if (!string.IsNullOrWhiteSpace(request.FileExtension))
        {
            query = query.Where(d => d.FileExtension == request.FileExtension.ToLower());
        }

        // Tags filter
        if (!string.IsNullOrWhiteSpace(request.Tags))
        {
            query = query.Where(d => d.Tags != null && d.Tags.Contains(request.Tags));
        }

        // Date range filter
        if (request.FromDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(d => d.CreatedAt <= request.ToDate.Value);
        }

        // Pagination
        var documents = await query
            .OrderByDescending(d => d.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<DocumentDto>>(documents);
    }
}

public class DeleteDocumentHandler : IRequestHandler<DeleteDocumentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public DeleteDocumentHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _context.Documents
            .FirstOrDefaultAsync(d => d.DocumentId == request.DocumentId, cancellationToken);

        if (document == null)
        {
            return false;
        }

        if (request.PermanentDelete)
        {
            // Permanent delete - remove from database
            _context.Documents.Remove(document);
        }
        else
        {
            // Soft delete - mark as deleted
            document.DeletedAt = DateTime.UtcNow;
            document.DeletedBy = _currentUserService.UserId;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}