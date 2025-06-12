using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Trainings.Commands;
using MaproSSO.Application.Features.Trainings.DTOs;
using MaproSSO.Domain.Entities.Trainings;
using MaproSSO.Application.Features.Trainings.Queries;

namespace MaproSSO.Application.Features.Trainings.Handlers;

public class CreateTrainingHandler : IRequestHandler<CreateTrainingCommand, TrainingDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public CreateTrainingHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<TrainingDto> Handle(CreateTrainingCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;

        // Generate training code
        var year = request.ScheduledDate.Year;
        var count = await _context.Trainings
            .Where(t => t.TenantId == tenantId && t.ScheduledDate.Year == year)
            .CountAsync(cancellationToken);

        var trainingCode = $"TRA-{year}-{(count + 1):D4}";

        var training = new Training
        {
            TrainingId = Guid.NewGuid(),
            TenantId = tenantId,
            TrainingCode = trainingCode,
            Title = request.Title,
            Description = request.Description,
            TrainingType = request.TrainingType,
            Mode = request.Mode,
            InstructorUserId = request.InstructorUserId,
            ExternalInstructor = request.ExternalInstructor,
            AreaId = request.AreaId,
            ScheduledDate = request.ScheduledDate,
            Duration = request.Duration,
            Location = request.Location,
            MaxParticipants = request.MaxParticipants,
            Status = "Scheduled",
            MaterialUrl = request.MaterialUrl,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.Trainings.Add(training);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.Trainings
            .Include(t => t.Instructor)
            .Include(t => t.Area)
            .Include(t => t.Participants)
                .ThenInclude(p => p.User)
            .FirstAsync(t => t.TrainingId == training.TrainingId, cancellationToken);

        return _mapper.Map<TrainingDto>(result);
    }
}

public class RegisterParticipantHandler : IRequestHandler<RegisterParticipantCommand, TrainingParticipantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public RegisterParticipantHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<TrainingParticipantDto> Handle(RegisterParticipantCommand request, CancellationToken cancellationToken)
    {
        // Check if training exists and has capacity
        var training = await _context.Trainings
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.TrainingId == request.TrainingId, cancellationToken);

        if (training == null)
        {
            throw new ArgumentException("Training not found");
        }

        if (training.MaxParticipants.HasValue && training.Participants.Count >= training.MaxParticipants.Value)
        {
            throw new InvalidOperationException("Training is at maximum capacity");
        }

        // Check if user is already registered
        var existingParticipant = await _context.TrainingParticipants
            .FirstOrDefaultAsync(p => p.TrainingId == request.TrainingId && p.UserId == request.UserId, cancellationToken);

        if (existingParticipant != null)
        {
            throw new InvalidOperationException("User is already registered for this training");
        }

        var participant = new TrainingParticipant
        {
            ParticipantId = Guid.NewGuid(),
            TrainingId = request.TrainingId,
            UserId = request.UserId,
            AttendanceStatus = "Registered",
            RegisteredAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId
        };

        _context.TrainingParticipants.Add(participant);
        await _context.SaveChangesAsync(cancellationToken);

        var result = await _context.TrainingParticipants
            .Include(p => p.User)
            .Include(p => p.Training)
            .FirstAsync(p => p.ParticipantId == participant.ParticipantId, cancellationToken);

        return _mapper.Map<TrainingParticipantDto>(result);
    }
}

public class TakeAttendanceHandler : IRequestHandler<TakeAttendanceCommand, List<TrainingParticipantDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public TakeAttendanceHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<List<TrainingParticipantDto>> Handle(TakeAttendanceCommand request, CancellationToken cancellationToken)
    {
        var participants = await _context.TrainingParticipants
            .Include(p => p.User)
            .Where(p => p.TrainingId == request.TrainingId)
            .ToListAsync(cancellationToken);

        foreach (var attendanceRecord in request.AttendanceRecords)
        {
            var participant = participants.FirstOrDefault(p => p.UserId == attendanceRecord.UserId);
            if (participant != null)
            {
                participant.AttendanceStatus = attendanceRecord.AttendanceStatus;
                participant.Score = attendanceRecord.Score;
                participant.Passed = attendanceRecord.Passed;
                participant.Comments = attendanceRecord.Comments;
                participant.AttendanceMarkedAt = DateTime.UtcNow;
                participant.UpdatedAt = DateTime.UtcNow;
                participant.UpdatedBy = _currentUserService.UserId;
            }
        }

        // Update training status to InProgress if not already
        var training = await _context.Trainings
            .FirstAsync(t => t.TrainingId == request.TrainingId, cancellationToken);

        if (training.Status == "Scheduled")
        {
            training.Status = "InProgress";
            training.UpdatedAt = DateTime.UtcNow;
            training.UpdatedBy = _currentUserService.UserId;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<List<TrainingParticipantDto>>(participants);
    }
}

public class GetTrainingByIdHandler : IRequestHandler<GetTrainingByIdQuery, TrainingDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTrainingByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TrainingDto?> Handle(GetTrainingByIdQuery request, CancellationToken cancellationToken)
    {
        var training = await _context.Trainings
            .Include(t => t.Instructor)
            .Include(t => t.Area)
            .Include(t => t.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.TrainingId == request.TrainingId, cancellationToken);

        return training != null ? _mapper.Map<TrainingDto>(training) : null;
    }
}