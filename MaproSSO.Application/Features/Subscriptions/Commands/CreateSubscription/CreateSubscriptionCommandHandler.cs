using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Services;
using MaproSSO.Domain.Entities.Subscription;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.ValueObjects;

namespace MaproSSO.Application.Features.Subscriptions.Commands.CreateSubscription
{
    public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, Result<SubscriptionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ISubscriptionService _subscriptionService;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<CreateSubscriptionCommandHandler> _logger;

        public CreateSubscriptionCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ISubscriptionService subscriptionService,
            ICurrentUserService currentUser,
            ILogger<CreateSubscriptionCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _subscriptionService = subscriptionService;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<SubscriptionDto>> Handle(
            CreateSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Verificar que el tenant existe
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == request.TenantId, cancellationToken);

                if (tenant == null)
                {
                    return Result<SubscriptionDto>.Failure("Empresa no encontrada");
                }

                // Verificar que no tenga una suscripción activa
                var activeSubscription = await _context.Subscriptions
                    .AnyAsync(s => s.TenantId == request.TenantId &&
                        (s.Status == SubscriptionStatus.Active || s.Status == SubscriptionStatus.Trial),
                        cancellationToken);

                if (activeSubscription)
                {
                    return Result<SubscriptionDto>.Failure("La empresa ya tiene una suscripción activa");
                }

                // Verificar que el plan existe
                var plan = await _context.Plans
                    .Include(p => p.Features)
                    .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.IsActive, cancellationToken);

                if (plan == null)
                {
                    return Result<SubscriptionDto>.Failure("Plan no válido");
                }

                var billingCycle = Enum.Parse<BillingCycle>(request.BillingCycle);
                Subscription subscription;

                if (request.StartTrial && plan.TrialDays > 0)
                {
                    // Crear suscripción de prueba
                    subscription = Subscription.CreateTrial(
                        request.TenantId,
                        request.PlanId,
                        plan.TrialDays,
                        _currentUser.UserId.Value);
                }
                else
                {
                    // Procesar pago
                    if (request.Payment == null)
                    {
                        return Result<SubscriptionDto>.Failure("Información de pago requerida");
                    }

                    // TODO: Integrar con pasarela de pagos
                    var paymentAmount = billingCycle == BillingCycle.Monthly
                        ? plan.MonthlyPrice
                        : plan.AnnualPrice;

                    var payment = Payment.Create(
                        Guid.NewGuid(), // SubscriptionId temporal
                        paymentAmount,
                        request.Payment.PaymentMethod,
                        GenerateInvoiceNumber());

                    // Simular pago exitoso
                    payment.MarkAsCompleted($"TRANS_{Guid.NewGuid()}", null);

                    // Crear suscripción pagada
                    subscription = Subscription.CreatePaid(
                        request.TenantId,
                        request.PlanId,
                        billingCycle,
                        Guid.NewGuid(), // PaymentMethodId
                        _currentUser.UserId.Value);

                    // Asociar el pago a la suscripción
                    var subscriptionPayment = Payment.Create(
                        subscription.Id,
                        paymentAmount,
                        request.Payment.PaymentMethod,
                        GenerateInvoiceNumber());

                    subscriptionPayment.MarkAsCompleted($"TRANS_{Guid.NewGuid()}", null);
                    _context.Payments.Add(subscriptionPayment);
                }

                _context.Subscriptions.Add(subscription);

                // Inicializar el uso de características
                foreach (var feature in plan.Features.Where(f => f.FeatureType == "Limit"))
                {
                    var limit = feature.GetFeatureLimit();
                    var featureUsage = FeatureUsage.Create(
                        request.TenantId,
                        feature.FeatureCode,
                        limit,
                        feature.FeatureCode == "storage_gb" ? "Never" : "Monthly");

                    _context.FeatureUsages.Add(featureUsage);
                }

                await _context.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<SubscriptionDto>(subscription);
                dto.PlanName = plan.PlanName;

                _logger.LogInformation(
                    "Suscripción creada: {SubscriptionId} para tenant {TenantId} con plan {PlanId}",
                    subscription.Id, request.TenantId, request.PlanId);

                return Result<SubscriptionDto>.Success(dto, "Suscripción creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear suscripción para tenant {TenantId}", request.TenantId);
                return Result<SubscriptionDto>.Failure("Error al crear la suscripción");
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}