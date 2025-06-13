using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Features.Tenants.Commands;
using MaproSSO.Application.Features.Tenants.DTOs;
using MaproSSO.Application.Features.Tenants.Queries;
using MaproSSO.Domain.Entities.Tenants;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Application.Common.Extensions;
using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Application.Features.Tenants.Handlers;

public class CreateTenantHandler : IRequestHandler<CreateTenantCommand, TenantDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher _passwordHasher;

    public CreateTenantHandler(
        IApplicationDbContext context,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Check if tenant with same email or tax ID already exists
        var existingTenant = await _context.Tenants
            .FirstOrDefaultAsync(t => t.Email == request.Email || t.TaxId == request.TaxId, cancellationToken);

        if (existingTenant != null)
        {
            throw new InvalidOperationException("A tenant with this email or tax ID already exists");
        }

        // Create tenant
        var tenant = new Tenant
        {
            TenantId = Guid.NewGuid(),
            CompanyName = request.CompanyName,
            TradeName = request.TradeName,
            TaxId = request.TaxId,
            Industry = request.Industry,
            EmployeeCount = request.EmployeeCount,
            Country = request.Country,
            State = request.State,
            City = request.City,
            Address = request.Address,
            PostalCode = request.PostalCode,
            Phone = request.Phone,
            Email = request.Email,
            Website = request.Website,
            LogoUrl = request.LogoUrl,
            TimeZone = request.TimeZone,
            Culture = request.Culture,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId.ToString()
        };

        _context.Tenants.Add(tenant);

        // Create admin user for the tenant
        var adminUser = new User
        {
            UserId = Guid.NewGuid(),
            TenantId = tenant.TenantId,
            Username = request.AdminUsername,
            Email = request.AdminEmail,
            NormalizedEmail = request.AdminEmail.ToUpperInvariant(),
            EmailConfirmed = true,
            PasswordHash = _passwordHasher.HashPassword(request.AdminPassword),
            SecurityStamp = Guid.NewGuid().ToString(),
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUserService.UserId.ToString()
        };

        _context.Users.Add(adminUser);

        // Assign admin role to user
        var adminRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.NormalizedRoleName == "ADMINSSO" && r.IsSystemRole, cancellationToken);

        if (adminRole != null)
        {
            var userRole = new UserRole
            {
                UserId = adminUser.UserId,
                RoleId = adminRole.RoleId,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = _currentUserService.UserId.ToString()
            };

            _context.UserRoles.Add(userRole);
        }

        // Create subscription if plan is specified
        if (request.PlanId.HasValue)
        {
            var plan = await _context.Plans.FirstOrDefaultAsync(p => p.PlanId == request.PlanId.Value, cancellationToken);
            if (plan != null)
            {
                var subscription = new Subscriptions.Subscription
                {
                    SubscriptionId = Guid.NewGuid(),
                    TenantId = tenant.TenantId,
                    PlanId = plan.PlanId,
                    BillingCycle = "Monthly",
                    Status = "Trial",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(30), // 30-day trial
                    TrialEndDate = DateTime.UtcNow.AddDays(30),
                    CurrentPeriodStart = DateTime.UtcNow,
                    CurrentPeriodEnd = DateTime.UtcNow.AddDays(30),
                    AutoRenew = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Subscriptions.Add(subscription);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TenantDto>(tenant);
    }
}

public class GetTenantByIdHandler : IRequestHandler<GetTenantByIdQuery, TenantDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTenantByIdHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TenantDto?> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tenants.AsQueryable();

        if (request.IncludeSettings)
        {
            query = query.Include(t => t.Settings);
        }

        var tenant = await query.FirstOrDefaultAsync(t => t.TenantId == request.TenantId, cancellationToken);

        if (tenant == null)
            return null;

        var tenantDto = _mapper.Map<TenantDto>(tenant);

        if (request.IncludeSubscription)
        {
            var subscription = await _context.Subscriptions
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(s => s.TenantId == request.TenantId && s.Status != "Cancelled", cancellationToken);

            if (subscription != null)
            {
                tenantDto.CurrentSubscription = _mapper.Map<SubscriptionDto>(subscription);
            }
        }

        if (request.IncludeStatistics)
        {
            tenantDto.Statistics = await GetTenantStatistics(request.TenantId, cancellationToken);
        }

        return tenantDto;
    }

    private async Task<TenantStatisticsDto> GetTenantStatistics(Guid tenantId, CancellationToken cancellationToken)
    {
        var totalUsers = await _context.Users.CountAsync(u => u.TenantId == tenantId && u.IsActive, cancellationToken);
        var activeUsers = await _context.Users.CountAsync(u => u.TenantId == tenantId && u.IsActive && u.LastLoginAt >= DateTime.UtcNow.AddDays(-30), cancellationToken);
        var totalAreas = await _context.Areas.CountAsync(a => a.TenantId == tenantId && a.IsActive, cancellationToken);

        var totalInspections = await _context.Inspections.CountAsync(i => i.TenantId == tenantId, cancellationToken);
        var totalAudits = await _context.Audits.CountAsync(a => a.TenantId == tenantId, cancellationToken);
        var totalAccidents = await _context.Accidents.CountAsync(a => a.TenantId == tenantId, cancellationToken);
        var totalTrainings = await _context.Trainings.CountAsync(t => t.TenantId == tenantId, cancellationToken);

        var documents = await _context.Documents
            .Where(d => d.TenantId == tenantId && d.IsCurrentVersion && d.DeletedAt == null)
            .ToListAsync(cancellationToken);

        var totalDocuments = documents.Count;
        var totalStorageUsed = documents.Sum(d => d.FileSizeBytes);

        var lastLogin = await _context.Users
            .Where(u => u.TenantId == tenantId && u.LastLoginAt.HasValue)
            .OrderByDescending(u => u.LastLoginAt)
            .Select(u => new { u.LastLoginAt, Name = u.FirstName + " " + u.LastName })
            .FirstOrDefaultAsync(cancellationToken);

        return new TenantStatisticsDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalAreas = totalAreas,
            TotalInspections = totalInspections,
            TotalAudits = totalAudits,
            TotalAccidents = totalAccidents,
            TotalTrainings = totalTrainings,
            TotalDocuments = totalDocuments,
            TotalStorageUsed = totalStorageUsed,
            TotalStorageFormatted = totalStorageUsed.FormatFileSize(),
            LastLoginDate = lastLogin?.LastLoginAt ?? DateTime.MinValue,
            LastLoginUser = lastLogin?.Name ?? "N/A",
            ModuleUsage = new Dictionary<string, int>
            {
                ["Inspections"] = totalInspections,
                ["Audits"] = totalAudits,
                ["Accidents"] = totalAccidents,
                ["Trainings"] = totalTrainings,
                ["Documents"] = totalDocuments
            }
        };
    }
}

public class GetTenantsHandler : IRequestHandler<GetTenantsQuery, List<TenantListDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTenantsHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<TenantListDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tenants.AsQueryable();

        // Apply filters
        if (request.IsActive.HasValue)
        {
            query = query.Where(t => t.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(request.Industry))
        {
            query = query.Where(t => t.Industry == request.Industry);
        }

        //if (!string.IsNullOrEmpty(request.Country))
        //{
        //    query = query.Where(t => t.Country == request.Country);
        //}

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(t =>
                t.CompanyName.ToLower().Contains(searchTerm) ||
                (t.TradeName != null && t.TradeName.ToLower().Contains(searchTerm)) ||
                t.Email.Value.ToLower().Contains(searchTerm) ||
                t.TaxId.ToLower().Contains(searchTerm));
        }

        // Apply sorting
        query = request.SortBy.ToLower() switch
        {
            "companyname" => request.SortDescending ? query.OrderByDescending(t => t.CompanyName) : query.OrderBy(t => t.CompanyName),
            "email" => request.SortDescending ? query.OrderByDescending(t => t.Email) : query.OrderBy(t => t.Email),
            "createdat" => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt),
            "industry" => request.SortDescending ? query.OrderByDescending(t => t.Industry) : query.OrderBy(t => t.Industry),
            _ => query.OrderBy(t => t.CompanyName)
        };

        // Apply pagination
        var tenants = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var tenantDtos = _mapper.Map<List<TenantListDto>>(tenants);

        // Get subscription info for each tenant
        var tenantIds = tenants.Select(t => t.Id).ToList();
        var subscriptions = await _context.Subscriptions
            .Include(s => s.Plan)
            .Where(s => tenantIds.Contains(s.TenantId) && s.Status != Domain.Enums.SubscriptionStatus.Cancelled)
            .ToListAsync(cancellationToken);

        foreach (var tenantDto in tenantDtos)
        {
            var subscription = subscriptions.FirstOrDefault(s => s.TenantId == tenantDto.TenantId);
            if (subscription != null)
            {
                tenantDto.SubscriptionPlan = subscription.Plan.PlanName;
                tenantDto.SubscriptionStatus = subscription.Status;
                tenantDto.SubscriptionEndDate = subscription.EndDate;
            }
        }

        return tenantDtos;
    }
}