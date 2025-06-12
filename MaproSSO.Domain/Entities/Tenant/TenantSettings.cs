using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.Tenant
{
    public class TenantSettings : BaseEntity
    {
        public Guid TenantId { get; private set; }
        public string SettingKey { get; private set; }
        public string SettingValue { get; private set; }
        public string DataType { get; private set; }
        public string Description { get; private set; }
        public bool IsEncrypted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        private TenantSettings() { }

        public static TenantSettings Create(
            Guid tenantId,
            string key,
            string value,
            string dataType,
            bool isEncrypted = false,
            string description = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new DomainException("La clave de configuración es requerida");

            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("El valor de configuración es requerido");

            return new TenantSettings
            {
                TenantId = tenantId,
                SettingKey = key,
                SettingValue = value,
                DataType = dataType,
                IsEncrypted = isEncrypted,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateValue(string newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
                throw new DomainException("El valor de configuración es requerido");

            SettingValue = newValue;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}