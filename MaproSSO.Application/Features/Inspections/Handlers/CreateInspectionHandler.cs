using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Inspections.Commands;
using MaproSSO.Application.Features.Inspections.DTOs;
using MaproSSO.Domain.Entities.Inspections;
using MaproSSO.Application.Features.Inspections.Queries;

namespace MaproSSO.Application.Features.Inspections.Handlers;

public class CreateInspectionHandler : IRequestHandler<CreateInspectionCommand, InspectionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateInspectionHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<InspectionDto> Handle(CreateInspectionCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Generate inspection code
        var year = request.ScheduledDate.Year;
        var count = await _context.Inspections
            .Where(i => i.TenantId == tenantId && i.ScheduledDate.Year == year)
            .CountAsync(cancellationToken);

        var inspectionCode = $"INS-{year}-{(count + 1):D4}";

        var inspection = new Inspection
        {
            InspectionId = Guid.NewGuid(),
            TenantId = tenantId,
            ProgramId = request.ProgramId,
            AreaId = request.AreaId,
            InspectionCode = inspectionCode,
            Title = request.Title,
            Description = request.Description,
            InspectorUserId = request.InspectorUserId,
            ScheduledDate = request.ScheduledDate,
            Status = "Pending",
            CompletionPercentage = 0,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Inspections.Add(inspection);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Inspections
            .Include(i => i.Program)
            .Include(i => i.Area)
            .Include(i => i.Inspector)
            .FirstAsync(i => i.InspectionId == inspection.InspectionId, cancellationToken);

        return _mapper.Map<InspectionDto>(result);
    }
}

public class UpdateInspectionHandler : IRequestHandler<UpdateInspectionCommand, InspectionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public UpdateInspectionHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<InspectionDto> Handle(UpdateInspectionCommand request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .FirstOrDefaultAsync(i => i.InspectionId == request.InspectionId, cancellationToken);

        if (inspection == null)
        {
            throw new ArgumentException("Inspection not found");
        }

        inspection.Title = request.Title;
        inspection.Description = request.Description;
        inspection.InspectorUserId = request.InspectorUserId;
        inspection.ScheduledDate = request.ScheduledDate;
        inspection.Status = request.Status;
        inspection.CompletionPercentage = request.CompletionPercentage;
        inspection.UpdatedAt = DateTime.UtcNow;
        inspection.UpdatedBy = _currentUserService.UserId;

        if (request.Status == "Completed" && inspection.ExecutedDate == null)
        {
            inspection.ExecutedDate = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Inspections
            .Include(i => i.Program)
            .Include(i => i.Area)
            .Include(i => i.Inspector)
            .FirstAsync(i => i.InspectionId == inspection.InspectionId, cancellationToken);

        return _mapper.Map<InspectionDto>(result);
    }
}

public class GetInspectionByIdHandler : IRequestHandler<GetInspectionByIdQuery, InspectionDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInspectionByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InspectionDto?> Handle(GetInspectionByIdQuery request, CancellationToken cancellationToken)
    {
        var inspection = await _context.Inspections
            .Include(i => i.Program)
            .Include(i => i.Area)
            .Include(i => i.Inspector)
            .Include(i => i.Observations)
                .ThenInclude(o => o.ResponsibleUser)
            .Include(i => i.Observations)
                .ThenInclude(o => o.Images)
            .Include(i => i.Observations)
                .ThenInclude(o => o.Evidences)
                    .ThenInclude(e => e.UploadedByUser)
            .FirstOrDefaultAsync(i => i.InspectionId == request.InspectionId, cancellationToken);

        return inspection != null ? _mapper.Map<InspectionDto>(inspection) : null;
    }
}