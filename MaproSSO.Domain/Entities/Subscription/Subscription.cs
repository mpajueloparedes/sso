using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Events.Subscription;
using MaproSSO.Domain.Exceptions;
using System.Numerics;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class Subscription : BaseAuditableEntity, IAggregateRoot
    {
        private readonly List<SubscriptionHistory> _history = new();
        private readonly List<Payment> _payments = new();

        public Guid TenantId { get; private set; }
        public Guid PlanId { get; private set; }
        public BillingCycle BillingCycle { get; private set; }
        public SubscriptionStatus Status { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime? TrialEndDate { get; private set; }
        public DateTime? NextBillingDate { get; private set; }
        public DateTime? CancellationDate { get; private set; }
        public DateTime? GracePeriodEndDate { get; private set; }
        public bool AutoRenew { get; private set; }
        public Guid? PaymentMethodId { get; private set; }
        public DateTime CurrentPeriodStart { get; private set; }
        public DateTime CurrentPeriodEnd { get; private set; }

        public virtual Plan Plan { get; private set; }
        public IReadOnlyCollection<SubscriptionHistory> History => _history.AsReadOnly();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private Subscription() { }

        public static Subscription CreateTrial(
            Guid tenantId,
            Guid planId,
            int trialDays,
            Guid createdBy)
        {
            if (trialDays <= 0)
                throw new DomainException("Los días de prueba deben ser mayor a cero");

            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(trialDays);

            var subscription = new Subscription
            {
                TenantId = tenantId,
                PlanId = planId,
                BillingCycle = BillingCycle.Monthly,
                Status = SubscriptionStatus.Trial,
                StartDate = startDate,
                EndDate = endDate,
                TrialEndDate = endDate,
                CurrentPeriodStart = startDate,
                CurrentPeriodEnd = endDate,
                AutoRenew = true
            };

            subscription.SetCreatedInfo(createdBy);
            subscription.AddDomainEvent(new SubscriptionCreatedEvent(
                subscription.Id, tenantId, planId, SubscriptionStatus.Trial));

            subscription._history.Add(SubscriptionHistory.Create(
                subscription.Id, "Created", null, planId, null, BillingCycle.Monthly.ToString(), createdBy));

            return subscription;
        }

        public static Subscription CreatePaid(
            Guid tenantId,
            Guid planId,
            BillingCycle billingCycle,
            Guid paymentMethodId,
            Guid createdBy)
        {
            var startDate = DateTime.UtcNow;
            var endDate = billingCycle == BillingCycle.Monthly
                ? startDate.AddMonths(1)
                : startDate.AddYears(1);

            var subscription = new Subscription
            {
                TenantId = tenantId,
                PlanId = planId,
                BillingCycle = billingCycle,
                Status = SubscriptionStatus.Active,
                StartDate = startDate,
                EndDate = endDate,
                NextBillingDate = endDate,
                PaymentMethodId = paymentMethodId,
                CurrentPeriodStart = startDate,
                CurrentPeriodEnd = endDate,
                AutoRenew = true
            };

            subscription.SetCreatedInfo(createdBy);
            subscription.AddDomainEvent(new SubscriptionCreatedEvent(
                subscription.Id, tenantId, planId, SubscriptionStatus.Active));

            subscription._history.Add(SubscriptionHistory.Create(
                subscription.Id, "Created", null, planId, null, billingCycle.ToString(), createdBy));

            return subscription;
        }

        public void Activate(Payment payment, Guid userId)
        {
            if (Status != SubscriptionStatus.Trial)
                throw new BusinessRuleValidationException("Solo las suscripciones en trial pueden ser activadas");

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            Status = SubscriptionStatus.Active;
            StartDate = DateTime.UtcNow;
            EndDate = BillingCycle == BillingCycle.Monthly
                ? StartDate.AddMonths(1)
                : StartDate.AddYears(1);
            NextBillingDate = EndDate;
            CurrentPeriodStart = StartDate;
            CurrentPeriodEnd = EndDate;

            _payments.Add(payment);
            SetUpdatedInfo(userId);

            _history.Add(SubscriptionHistory.Create(
                Id, "Activated", PlanId, PlanId, BillingCycle.ToString(), BillingCycle.ToString(), userId));
        }

        public void Renew(Payment payment, Guid userId)
        {
            if (Status != SubscriptionStatus.Active && Status != SubscriptionStatus.Suspended)
                throw new BusinessRuleValidationException("Solo las suscripciones activas o suspendidas pueden ser renovadas");

            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            Status = SubscriptionStatus.Active;
            StartDate = DateTime.UtcNow;
            EndDate = BillingCycle == BillingCycle.Monthly
                ? StartDate.AddMonths(1)
                : StartDate.AddYears(1);
            NextBillingDate = EndDate;
            CurrentPeriodStart = StartDate;
            CurrentPeriodEnd = EndDate;
            GracePeriodEndDate = null;

            _payments.Add(payment);
            SetUpdatedInfo(userId);

            AddDomainEvent(new SubscriptionRenewedEvent(Id, TenantId, EndDate));

            _history.Add(SubscriptionHistory.Create(
                Id, "Renewed", PlanId, PlanId, BillingCycle.ToString(), BillingCycle.ToString(), userId));
        }

        public void Suspend(string reason, Guid userId)
        {
            if (Status != SubscriptionStatus.Active)
                throw new BusinessRuleValidationException("Solo las suscripciones activas pueden ser suspendidas");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón de suspensión es requerida");

            Status = SubscriptionStatus.Suspended;
            GracePeriodEndDate = DateTime.UtcNow.AddDays(7);
            SetUpdatedInfo(userId);

            AddDomainEvent(new SubscriptionSuspendedEvent(Id, TenantId, reason, GracePeriodEndDate.Value));

            _history.Add(SubscriptionHistory.Create(
                Id, "Suspended", PlanId, PlanId, BillingCycle.ToString(), BillingCycle.ToString(), userId, reason));
        }

        public void Cancel(string reason, Guid userId)
        {
            if (Status == SubscriptionStatus.Cancelled || Status == SubscriptionStatus.Expired)
                throw new BusinessRuleValidationException("La suscripción ya está cancelada o expirada");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("La razón de cancelación es requerida");

            Status = SubscriptionStatus.Cancelled;
            CancellationDate = DateTime.UtcNow;
            AutoRenew = false;
            SetUpdatedInfo(userId);

            _history.Add(SubscriptionHistory.Create(
                Id, "Cancelled", PlanId, PlanId, BillingCycle.ToString(), BillingCycle.ToString(), userId, reason));
        }

        public void Expire()
        {
            if (Status != SubscriptionStatus.Suspended)
                throw new BusinessRuleValidationException("Solo las suscripciones suspendidas pueden expirar");

            if (GracePeriodEndDate > DateTime.UtcNow)
                throw new BusinessRuleValidationException("La suscripción aún está en período de gracia");

            Status = SubscriptionStatus.Expired;
            SetUpdatedInfo(Guid.Empty); // Sistema

            _history.Add(SubscriptionHistory.Create(
                Id, "Expired", PlanId, PlanId, BillingCycle.ToString(), BillingCycle.ToString(), Guid.Empty));
        }

        public void ChangePlan(Guid newPlanId, BillingCycle newBillingCycle, Guid userId)
        {
            if (Status != SubscriptionStatus.Active)
                throw new BusinessRuleValidationException("Solo las suscripciones activas pueden cambiar de plan");

            var oldPlanId = PlanId;
            var oldBillingCycle = BillingCycle;

            PlanId = newPlanId;
            BillingCycle = newBillingCycle;
            SetUpdatedInfo(userId);

            _history.Add(SubscriptionHistory.Create(
                Id, "PlanChanged", oldPlanId, newPlanId, oldBillingCycle.ToString(), newBillingCycle.ToString(), userId));
        }

        public void ToggleAutoRenew(Guid userId)
        {
            AutoRenew = !AutoRenew;
            SetUpdatedInfo(userId);
        }

        public bool IsInGracePeriod()
        {
            return Status == SubscriptionStatus.Suspended &&
                   GracePeriodEndDate.HasValue &&
                   GracePeriodEndDate.Value > DateTime.UtcNow;
        }

        public bool CanAccess()
        {
            return Status == SubscriptionStatus.Trial ||
                   Status == SubscriptionStatus.Active ||
                   IsInGracePeriod();
        }

        public int GetDaysRemaining()
        {
            if (Status == SubscriptionStatus.Expired || Status == SubscriptionStatus.Cancelled)
                return 0;

            var targetDate = IsInGracePeriod() ? GracePeriodEndDate.Value : EndDate;
            var days = (int)(targetDate - DateTime.UtcNow).TotalDays;

            return Math.Max(0, days);
        }
    }
}