using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.Entities.Subscription
{
    public class PlanFeature : BaseEntity
    {
        public Guid PlanId { get; private set; }
        public string FeatureName { get; private set; }
        public string FeatureCode { get; private set; }
        public string FeatureType { get; private set; } // Module, Limit, Feature
        public string Value { get; private set; }
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsEnabled { get; private set; }

        private PlanFeature() { }

        public static PlanFeature Create(
            Guid planId,
            string featureName,
            string featureCode,
            string featureType,
            string value,
            string description = null,
            int sortOrder = 0)
        {
            if (string.IsNullOrWhiteSpace(featureName))
                throw new DomainException("El nombre de la característica es requerido");

            if (string.IsNullOrWhiteSpace(featureCode))
                throw new DomainException("El código de la característica es requerido");

            if (string.IsNullOrWhiteSpace(featureType))
                throw new DomainException("El tipo de característica es requerido");

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El valor de la característica es requerido");

            return new PlanFeature
            {
                PlanId = planId,
                FeatureName = featureName,
                FeatureCode = featureCode,
                FeatureType = featureType,
                Value = value,
                Description = description,
                SortOrder = sortOrder,
                IsEnabled = true
            };
        }

        public void UpdateValue(string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                throw new DomainException("El valor de la característica es requerido");

            Value = newValue;
        }

        public void Enable()
        {
            IsEnabled = true;
        }

        public void Disable()
        {
            IsEnabled = false;
        }

        public bool IsModuleFeature() => FeatureType == "Module";
        public bool IsLimitFeature() => FeatureType == "Limit";
        public bool IsGeneralFeature() => FeatureType == "Feature";
    }
}
