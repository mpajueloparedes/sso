using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class FeatureUsage : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public string FeatureCode { get; private set; }
        public int CurrentUsage { get; private set; }
        public int? UsageLimit { get; private set; }
        public string ResetPeriod { get; private set; } // Daily, Monthly, Annual, Never
        public DateTime? LastResetDate { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private FeatureUsage() { }

        public static FeatureUsage Create(
            Guid tenantId,
            string featureCode,
            int? usageLimit = null,
            string resetPeriod = "Never")
        {
            if (string.IsNullOrWhiteSpace(featureCode))
                throw new DomainException("El código de característica es requerido");

            return new FeatureUsage
            {
                TenantId = tenantId,
                FeatureCode = featureCode,
                CurrentUsage = 0,
                UsageLimit = usageLimit,
                ResetPeriod = resetPeriod,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public void IncrementUsage(int increment = 1)
        {
            if (increment <= 0)
                throw new DomainException("El incremento debe ser mayor a cero");

            if (UsageLimit.HasValue && (CurrentUsage + increment) > UsageLimit.Value)
                throw new BusinessRuleValidationException($"Se ha alcanzado el límite de uso para {FeatureCode}");

            CurrentUsage += increment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementUsage(int decrement = 1)
        {
            if (decrement <= 0)
                throw new DomainException("El decremento debe ser mayor a cero");

            if ((CurrentUsage - decrement) < 0)
                throw new BusinessRuleValidationException("El uso no puede ser negativo");

            CurrentUsage -= decrement;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ResetUsage()
        {
            CurrentUsage = 0;
            LastResetDate = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLimit(int? newLimit)
        {
            UsageLimit = newLimit;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool ShouldReset()
        {
            if (ResetPeriod == "Never") return false;
            if (!LastResetDate.HasValue) return true;

            var now = DateTime.UtcNow;
            return ResetPeriod switch
            {
                "Daily" => now.Date > LastResetDate.Value.Date,
                "Monthly" => now.Month != LastResetDate.Value.Month || now.Year != LastResetDate.Value.Year,
                "Annual" => now.Year != LastResetDate.Value.Year,
                _ => false
            };
        }

        public int GetRemainingUsage()
        {
            if (!UsageLimit.HasValue) return int.MaxValue;
            return Math.Max(0, UsageLimit.Value - CurrentUsage);
        }

        public decimal GetUsagePercentage()
        {
            if (!UsageLimit.HasValue || UsageLimit.Value == 0) return 0;
            return (CurrentUsage * 100m) / UsageLimit.Value;
        }
    }
}