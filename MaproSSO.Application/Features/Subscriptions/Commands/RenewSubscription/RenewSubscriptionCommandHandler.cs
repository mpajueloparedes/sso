using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using MaproSSO.Application.Common.Interfaces;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Exceptions;
using MaproSSO.Domain.Entities.Subscription;
using MaproSSO.Domain.Enums;
using MaproSSO.Application.Common.Services;

namespace MaproSSO.Application.Features.Subscriptions.Commands.RenewSubscription
{
    public class RenewSubscriptionCommandHandler : IRequestHandler<RenewSubscriptionCommand, Result<SubscriptionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<RenewSubscriptionCommandHandler> _logger;

        public RenewSubscriptionCommandHandler(
            IApplicationDbContext context,
            IMapper mapper,
            ICurrentUserService currentUser,
            ILogger<RenewSubscriptionCommandHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<Result<SubscriptionDto>> Handle(
            RenewSubscriptionCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                var subscription = await _context.Subscriptions
                    .Include(s => s.Plan)
                    .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, cancellationToken);

                if (subscription == null)
                {
                    throw new NotFoundException(nameof(Subscription), request.SubscriptionId);
                }

                // Verificar permisos
                if (!_currentUser.IsSuperAdmin && subscription.TenantId != _currentUser.TenantId)
                {
                    throw new ForbiddenAccessException("No tiene permisos para renovar esta suscripción");
                }

                if (subscription.Status != SubscriptionStatus.Active &&
                    subscription.Status != SubscriptionStatus.Suspended)
                {
                    return Result<SubscriptionDto>.Failure("Solo se pueden renovar suscripciones activas o suspendidas");
                }

                // Calcular monto del pago
                var paymentAmount = subscription.BillingCycle == BillingCycle.Monthly
                    ? subscription.Plan.MonthlyPrice
                    : subscription.Plan.AnnualPrice;

                // Procesar pago
                var payment = Payment.Create(
                    subscription.Id,
                    paymentAmount,
                    request.Payment.PaymentMethod,
                    GenerateInvoiceNumber());

                // TODO: Integrar con pasarela de pagos
                payment.MarkAsCompleted($"TRANS_{Guid.NewGuid()}", null);

                // Renovar suscripción
                subscription.Renew(payment, _currentUser.UserId.Value);

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync(cancellationToken);

                var dto = _mapper.Map<SubscriptionDto>(subscription);
                dto.PlanName = subscription.Plan.PlanName;

                _logger.LogInformation(
                    "Suscripción renovada: {SubscriptionId} por usuario {UserId}",
                    subscription.Id, _currentUser.UserId);

                return Result<SubscriptionDto>.Success(dto, "Suscripción renovada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al renovar suscripción {SubscriptionId}", request.SubscriptionId);
                throw;
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMM}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}