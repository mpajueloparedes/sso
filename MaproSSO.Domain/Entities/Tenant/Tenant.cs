using MaproSSO.Domain.Common;
using MaproSSO.Domain.ValueObjects;
using MaproSSO.Domain.Events.Tenant;
using MaproSSO.Domain.Exceptions;
using System.Net;

namespace MaproSSO.Domain.Entities.Tenant
{
    public class Tenant : BaseAggregateRoot, IAggregateRoot
    {
        private readonly List<TenantSettings> _settings = new();

        public string CompanyName { get; private set; }
        public string TradeName { get; private set; }
        public string TaxId { get; private set; }
        public string Industry { get; private set; }
        public int EmployeeCount { get; private set; }
        public Address Address { get; private set; }
        public string Phone { get; private set; }
        public Email Email { get; private set; }
        public string Website { get; private set; }
        public string LogoUrl { get; private set; }
        public string TimeZone { get; private set; }
        public string Culture { get; private set; }
        public bool IsActive { get; private set; }

        public IReadOnlyCollection<TenantSettings> Settings => _settings.AsReadOnly();

        private Tenant() { }

        public static Tenant Create(
            string companyName,
            string taxId,
        string industry,
            Address address,
            string phone,
            Email email,
            Guid createdBy)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new DomainException("El nombre de la empresa es requerido");

            if (string.IsNullOrWhiteSpace(taxId))
                throw new DomainException("El RUC/DNI es requerido");

            if (string.IsNullOrWhiteSpace(industry))
                throw new DomainException("La industria es requerida");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("El teléfono es requerido");

            var tenant = new Tenant
            {
                CompanyName = companyName,
                TaxId = taxId,
                Industry = industry,
                Address = address ?? throw new ArgumentNullException(nameof(address)),
                Phone = phone,
                Email = email ?? throw new ArgumentNullException(nameof(email)),
                EmployeeCount = 0,
                TimeZone = "America/Lima",
                Culture = "es-PE",
                IsActive = true
            };

            tenant.SetCreatedInfo(createdBy);
            tenant.AddDomainEvent(new TenantCreatedEvent(tenant.Id, companyName, email.Value));

            return tenant;
        }

        public void Update(
            string companyName,
            string tradeName,
            string industry,
            int employeeCount,
            Address address,
            string phone,
            string website,
            Guid updatedBy)
        {
            CompanyName = companyName;
            TradeName = tradeName;
            Industry = industry;
            EmployeeCount = employeeCount;
            Address = address;
            Phone = phone;
            Website = website;

            SetUpdatedInfo(updatedBy);
        }

        public void UpdateLogo(string logoUrl, Guid updatedBy)
        {
            LogoUrl = logoUrl;
            SetUpdatedInfo(updatedBy);
        }

        public void UpdateCulture(string timeZone, string culture, Guid updatedBy)
        {
            TimeZone = timeZone;
            Culture = culture;
            SetUpdatedInfo(updatedBy);
        }

        public void Deactivate(Guid userId)
        {
            if (!IsActive)
                throw new BusinessRuleValidationException("El tenant ya está inactivo");

            IsActive = false;
            SetUpdatedInfo(userId);

            AddDomainEvent(new TenantDeactivatedEvent(Id, CompanyName));
        }

        public void Activate(Guid userId)
        {
            if (IsActive)
                throw new BusinessRuleValidationException("El tenant ya está activo");

            IsActive = true;
            SetUpdatedInfo(userId);
        }

        public void AddSetting(string key, string value, string dataType, bool isEncrypted = false, string description = null)
        {
            var existingSetting = _settings.FirstOrDefault(s => s.SettingKey == key);
            if (existingSetting != null)
            {
                existingSetting.UpdateValue(value);
            }
            else
            {
                _settings.Add(TenantSettings.Create(Id, key, value, dataType, isEncrypted, description));
            }
        }

        public string GetSetting(string key)
        {
            return _settings.FirstOrDefault(s => s.SettingKey == key)?.SettingValue;
        }

        public void RemoveSetting(string key)
        {
            var setting = _settings.FirstOrDefault(s => s.SettingKey == key);
            if (setting != null)
            {
                _settings.Remove(setting);
            }
        }
    }
}