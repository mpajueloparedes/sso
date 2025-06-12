using MaproSSO.Domain.Common;
using MaproSSO.Domain.Enums;
using MaproSSO.Domain.Exceptions;
using MaproSSO.Domain.ValueObjects;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class Plan : BaseEntity
    {
        private readonly List<PlanFeature> _features = new();

        public string PlanName { get; private set; }
        public PlanType PlanType { get; private set; }
        public string Description { get; private set; }
        public Money MonthlyPrice { get; private set; }
        public Money AnnualPrice { get; private set; }
        public int TrialDays { get; private set; }
        public bool IsActive { get; private set; }
        public int SortOrder { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyCollection<PlanFeature> Features => _features.AsReadOnly();

        private Plan() { }

        public static Plan Create(
            string planName,
            PlanType planType,
            Money monthlyPrice,
            Money annualPrice,
            string description = null,
            int trialDays = 30)
        {
            if (string.IsNullOrWhiteSpace(planName))
                throw new DomainException("El nombre del plan es requerido");

            if (monthlyPrice == null)
                throw new ArgumentNullException(nameof(monthlyPrice));

            if (annualPrice == null)
                throw new ArgumentNullException(nameof(annualPrice));

            if (annualPrice.Amount > monthlyPrice.Amount * 12)
                throw new BusinessRuleValidationException("El precio anual no puede ser mayor que 12 meses del precio mensual");

            if (trialDays < 0)
                throw new BusinessRuleValidationException("Los días de prueba no pueden ser negativos");

            return new Plan
            {
                PlanName = planName,
                PlanType = planType,
                Description = description,
                MonthlyPrice = monthlyPrice,
                AnnualPrice = annualPrice,
                TrialDays = trialDays,
                IsActive = true,
                SortOrder = (int)planType,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(
            string planName,
            string description,
            Money monthlyPrice,
            Money annualPrice,
            int trialDays)
        {
            PlanName = planName;
            Description = description;
            MonthlyPrice = monthlyPrice;
            AnnualPrice = annualPrice;
            TrialDays = trialDays;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddFeature(string name, string code, string featureType, string value, string description = null, int sortOrder = 0)
        {
            if (_features.Any(f => f.FeatureCode == code))
                throw new BusinessRuleValidationException($"La característica {code} ya existe en el plan");

            _features.Add(PlanFeature.Create(Id, name, code, featureType, value, description, sortOrder));
        }

        public void RemoveFeature(string featureCode)
        {
            var feature = _features.FirstOrDefault(f => f.FeatureCode == featureCode);
            if (feature != null)
            {
                _features.Remove(feature);
            }
        }

        public void UpdateFeature(string featureCode, string value)
        {
            var feature = _features.FirstOrDefault(f => f.FeatureCode == featureCode);
            if (feature == null)
                throw new EntityNotFoundException($"Característica {featureCode} no encontrada");

            feature.UpdateValue(value);
        }

        public bool HasFeature(string featureCode)
        {
            return _features.Any(f => f.FeatureCode == featureCode && f.IsEnabled);
        }

        public string GetFeatureValue(string featureCode)
        {
            return _features.FirstOrDefault(f => f.FeatureCode == featureCode)?.Value;
        }

        public int? GetFeatureLimit(string featureCode)
        {
            var value = GetFeatureValue(featureCode);
            if (value == "unlimited") return null;
            if (int.TryParse(value, out var limit)) return limit;
            return 0;
        }

        public Money GetPrice(BillingCycle billingCycle)
        {
            return billingCycle == BillingCycle.Monthly ? MonthlyPrice : AnnualPrice;
        }

        public decimal GetMonthlyEquivalent(BillingCycle billingCycle)
        {
            return billingCycle == BillingCycle.Monthly
                ? MonthlyPrice.Amount
                : AnnualPrice.Amount / 12;
        }
    }
}