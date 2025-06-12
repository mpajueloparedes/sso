using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Application.Common.Services;
using MaproSSO.Domain.Entities.Tenant;
using MaproSSO.Domain.Entities.Security;
using MaproSSO.Domain.ValueObjects;
using MaproSSO.Domain.Enums;
using MaproSSO.Application.Features.Tenants.Dtos;
using MaproSSO.Domain.Entities.SSO;

namespace MaproSSO.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Result<TenantDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<CreateTenantCommandHandler> _logger;

        public CreateTenantCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            IPasswordHasher passwordHasher,
            ISubscriptionService subscriptionService,
            ICurrentUserService currentUser,
            ILogger<CreateTenantCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _subscriptionService = subscriptionService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<TenantDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar si ya existe un tenant con el mismo RUC
                var existingTenant = await _context.Tenants
                    .AnyAsync(t => t.TaxId == request.TaxId, cancellationToken);

                if (existingTenant)
                {
                    return Result<TenantDto>.Failure($"Ya existe una empresa registrada con el RUC {request.TaxId}");
                }

                // Verificar si el email de la empresa ya está en uso
                var existingEmail = await _context.Tenants
                    .AnyAsync(t => t.Email.Value == request.Email.ToLowerInvariant(), cancellationToken);

                if (existingEmail)
                {
                    return Result<TenantDto>.Failure($"El email {request.Email} ya está en uso");
                }

                // Verificar si el email del administrador ya está en uso
                var existingAdminEmail = await _context.Users
                    .AnyAsync(u => u.NormalizedEmail == request.AdminEmail.ToUpperInvariant(), cancellationToken);

                if (existingAdminEmail)
                {
                    return Result<TenantDto>.Failure($"El email del administrador {request.AdminEmail} ya está en uso");
                }

                // Verificar que el plan existe
                var plan = await _context.Plans
                    .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, cancellationToken);

                if (plan == null)
                {
                    return Result<TenantDto>.Failure("Plan de suscripción no válido");
                }

                // Crear tenant
                var address = Address.Create(
                    request.Address.Country,
                    request.Address.State,
                    request.Address.City,
                    request.Address.Street,
                    request.Address.PostalCode);

                var email = Email.Create(request.Email);

                var tenant = Tenant.Create(
                    request.CompanyName,
                    request.TaxId,
                    request.Industry,
                    address,
                    request.Phone,
                    email,
                    _currentUser.UserId.Value);

                tenant.Update(
                    request.CompanyName,
                    request.TradeName,
                    request.Industry,
                    request.EmployeeCount,
                    address,
                    request.Phone,
                    request.Website,
                    _currentUser.UserId.Value);

                _context.Tenants.Add(tenant);

                // Crear rol de administrador para el tenant
                var adminRole = Role.CreateTenantRole(
                    tenant.Id,
                    "Administrador SSO",
                    "Administrador del módulo de SSO del tenant");

                _context.Roles.Add(adminRole);

                // Asignar todos los permisos del módulo SSO al rol
                var ssoPermissions = await _context.Permissions
                    .Where(p => p.Module == "SSO" || p.Module == "Tenant")
                    .ToListAsync(cancellationToken);

                foreach (var permission in ssoPermissions)
                {
                    adminRole.AssignPermission(permission.Id, _currentUser.UserId.Value);
                }

                // Crear usuario administrador
                var adminEmail = Email.Create(request.AdminEmail);
                var passwordHash = _passwordHasher.HashPassword(request.AdminPassword);

                var adminUser = User.Create(
                    tenant.Id,
                    request.AdminUsername,
                    adminEmail,
                    request.AdminFirstName,
                    request.AdminLastName,
                    passwordHash,
                    _currentUser.UserId.Value);

                adminUser.ConfirmEmail(); // Pre-confirmar el email del admin
                adminUser.AssignRole(adminRole.Id, _currentUser.UserId.Value);

                _context.Users.Add(adminUser);

                // Crear suscripción
                var billingCycle = Enum.Parse<BillingCycle>(request.BillingCycle);

                if (plan.TrialDays > 0)
                {
                    // Crear suscripción de prueba
                    var trialResult = await _subscriptionService.CreateTrialSubscriptionAsync(
                        tenant.Id, request.PlanId);

                    if (!trialResult.Succeeded)
                    {
                        return Result<TenantDto>.Failure("Error al crear la suscripción de prueba");
                    }
                }
                else
                {
                    // Crear suscripción activa (requiere pago)
                    return Result<TenantDto>.Failure("La creación de suscripciones de pago aún no está implementada");
                }

                // Crear configuraciones iniciales del tenant
                tenant.AddSetting("theme", "light", "String", false, "Tema de la interfaz");
                tenant.AddSetting("language", "es-PE", "String", false, "Idioma predeterminado");
                tenant.AddSetting("dateFormat", "dd/MM/yyyy", "String", false, "Formato de fecha");
                tenant.AddSetting("timeFormat", "HH:mm", "String", false, "Formato de hora");

                // Crear áreas predeterminadas
                var areas = new[]
                {
                    ("ADMIN", "Administración"),
                    ("OPER", "Operaciones"),
                    ("MANT", "Mantenimiento"),
                    ("RRHH", "Recursos Humanos"),
                    ("SEG", "Seguridad")
                };

                foreach (var (code, name) in areas)
                {
                    var area = Area.Create(tenant.Id, name, code, $"Área de {name}", null, null, adminUser.Id);
                    _context.Areas.Add(area);
                }

                // Crear pilares predeterminados
                var pillars = new[]
                {
                    ("LIDER", "Liderazgo", "#FF5733"),
                    ("RIESGO", "Gestión de Riesgo", "#33FF57"),
                    ("LEGAL", "Cumplimiento Legal", "#3357FF"),
                    ("OBJETIVO", "Objetivos y Metas", "#FF33F5"),
                    ("COMPETENCIA", "Competencia y Formación", "#F5FF33"),
                    ("COMUNICACION", "Comunicación", "#33FFF5"),
                    ("OPERACION", "Control Operacional", "#FF8C33"),
                    ("EMERGENCIA", "Preparación ante Emergencias", "#8C33FF"),
                    ("MEJORA", "Mejora Continua", "#33FF8C")
                };

                var sortOrder = 0;
                foreach (var (code, name, color) in pillars)
                {
                    var pillar = Pillar.Create(tenant.Id, name, code, null, "fas fa-shield-alt", color, sortOrder++, adminUser.Id);
                    _context.Pillars.Add(pillar);
                }

                await _context.SaveChangesAsync(cancellationToken);

                var tenantDto = _mapper.Map<TenantDto>(tenant);

                _logger.LogInformation("Tenant creado exitosamente: {TenantId} - {CompanyName}",
                    tenant.Id, tenant.CompanyName);

                return Result<TenantDto>.Success(tenantDto, "Empresa creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear tenant");
                return Result<TenantDto>.Failure("Error al crear la empresa");
            }
        }
    }
}