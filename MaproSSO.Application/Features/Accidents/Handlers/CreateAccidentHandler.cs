using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Accidents.Commands;
using MaproSSO.Application.Features.Accidents.DTOs;
using MaproSSO.Domain.Entities.Accidents;
using MaproSSO.Application.Features.Accidents.Queries;

namespace MaproSSO.Application.Features.Accidents.Handlers;

public class CreateAccidentHandler : IRequestHandler<CreateAccidentCommand, AccidentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateAccidentHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AccidentDto> Handle(CreateAccidentCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Generate accident code
        var year = request.OccurredAt.Year;
        var count = await _context.Accidents
            .Where(a => a.TenantId == tenantId && a.OccurredAt.Year == year)
            .CountAsync(cancellationToken);

        var accidentCode = $"ACC-{year}-{(count + 1):D4}";

        var accident = new Accident
        {
            AccidentId = Guid.NewGuid(),
            TenantId = tenantId,
            AccidentCode = accidentCode,
            Type = request.Type,
            Severity = request.Severity,
            OccurredAt = request.OccurredAt,
            ReportedAt = DateTime.UtcNow,
            Shift = request.Shift,
            Location = request.Location,
            Description = request.Description,
            WitnessNames = request.WitnessNames,
            Status = "Reported",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Accidents.Add(accident);

        // Add people involved
        foreach (var personDto in request.People)
        {
            var person = new AccidentPerson
            {
                PersonId = Guid.NewGuid(),
                AccidentId = accident.AccidentId,
                PersonType = personDto.PersonType,
                UserId = personDto.UserId,
                FullName = personDto.FullName,
                DocumentNumber = personDto.DocumentNumber,
                Age = personDto.Age,
                Gender = personDto.Gender,
                Position = personDto.Position,
                Company = personDto.Company,
                AreaId = personDto.AreaId,
                InjuryType = personDto.InjuryType,
                InjuryDescription = personDto.InjuryDescription,
                MedicalAttention = personDto.MedicalAttention,
                LostWorkDays = personDto.LostWorkDays
            };

            _context.AccidentPeople.Add(person);
        }

        // Add images
        for (int i = 0; i < request.ImageUrls.Count; i++)
        {
            var image = new AccidentImage
            {
                ImageId = Guid.NewGuid(),
                AccidentId = accident.AccidentId,
                ImageUrl = request.ImageUrls[i],
                Description = $"Imagen del accidente {i + 1}",
                SortOrder = i
            };

            _context.AccidentImages.Add(image);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Accidents
            .Include(a => a.People)
                .ThenInclude(p => p.User)
            .Include(a => a.People)
                .ThenInclude(p => p.Area)
            .Include(a => a.Images)
            .FirstAsync(a => a.AccidentId == accident.AccidentId, cancellationToken);

        return _mapper.Map<AccidentDto>(result);
    }
}

public class StartInvestigationHandler : IRequestHandler<StartInvestigationCommand, AccidentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public StartInvestigationHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AccidentDto> Handle(StartInvestigationCommand request, CancellationToken cancellationToken)
    {
        var accident = await _context.Accidents
            .FirstOrDefaultAsync(a => a.AccidentId == request.AccidentId, cancellationToken);

        if (accident == null)
        {
            throw new ArgumentException("Accident not found");
        }

        accident.Status = "UnderInvestigation";
        accident.InvestigationStartDate = DateTime.UtcNow;
        accident.UpdatedAt = DateTime.UtcNow;
        accident.UpdatedBy = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Accidents
            .Include(a => a.People)
                .ThenInclude(p => p.User)
            .Include(a => a.People)
                .ThenInclude(p => p.Area)
            .Include(a => a.Images)
            .FirstAsync(a => a.AccidentId == accident.AccidentId, cancellationToken);

        return _mapper.Map<AccidentDto>(result);
    }
}

public class CloseInvestigationHandler : IRequestHandler<CloseInvestigationCommand, AccidentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CloseInvestigationHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<AccidentDto> Handle(CloseInvestigationCommand request, CancellationToken cancellationToken)
    {
        var accident = await _context.Accidents
            .FirstOrDefaultAsync(a => a.AccidentId == request.AccidentId, cancellationToken);

        if (accident == null)
        {
            throw new ArgumentException("Accident not found");
        }

        accident.Status = "Closed";
        accident.ImmediateCauses = request.ImmediateCauses;
        accident.RootCauses = request.RootCauses;
        accident.InvestigationEndDate = DateTime.UtcNow;
        accident.UpdatedAt = DateTime.UtcNow;
        accident.UpdatedBy = _currentUserService.UserId;

        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Accidents
            .Include(a => a.People)
                .ThenInclude(p => p.User)
            .Include(a => a.People)
                .ThenInclude(p => p.Area)
            .Include(a => a.Images)
            .FirstAsync(a => a.AccidentId == accident.AccidentId, cancellationToken);

        return _mapper.Map<AccidentDto>(result);
    }
}

public class GetAccidentByIdHandler : IRequestHandler<GetAccidentByIdQuery, AccidentDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAccidentByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<AccidentDto?> Handle(GetAccidentByIdQuery request, CancellationToken cancellationToken)
    {
        var accident = await _context.Accidents
            .Include(a => a.People)
                .ThenInclude(p => p.User)
            .Include(a => a.People)
                .ThenInclude(p => p.Area)
            .Include(a => a.Images)
            .FirstOrDefaultAsync(a => a.AccidentId == request.AccidentId, cancellationToken);

        return accident != null ? _mapper.Map<AccidentDto>(accident) : null;
    }
}