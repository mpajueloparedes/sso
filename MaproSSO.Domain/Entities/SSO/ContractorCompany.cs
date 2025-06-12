using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;
using MaproSSO.Domain.ValueObjects;
using System.Net;

namespace MaproSSO.Domain.Entities.SSO
{
    public class ContractorCompany : BaseAuditableEntity
    {
        public Guid TenantId { get; private set; }
        public string CompanyName { get; private set; }
        public string TaxId { get; private set; }
        public string ContactName { get; private set; }
        public Email ContactEmail { get; private set; }
        public string ContactPhone { get; private set; }
        public Address Address { get; private set; }
        public DateTime ContractStartDate { get; private set; }
        public DateTime? ContractEndDate { get; private set; }
        public bool IsActive { get; private set; }

        private ContractorCompany() { }

        public static ContractorCompany Create(
            Guid tenantId,
            string companyName,
            string taxId,
            string contactName,
            Email contactEmail,
            string contactPhone,
            Address address,
            DateTime contractStartDate,
            DateTime? contractEndDate,
            Guid createdBy)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new DomainException("El nombre de la empresa es requerido");

            if (string.IsNullOrWhiteSpace(taxId))
                throw new DomainException("El RUC/DNI es requerido");

            if (string.IsNullOrWhiteSpace(contactName))
                throw new DomainException("El nombre de contacto es requerido");

            if (string.IsNullOrWhiteSpace(contactPhone))
                throw new DomainException("El teléfono de contacto es requerido");

            if (contractEndDate.HasValue && contractEndDate.Value <= contractStartDate)
                throw new BusinessRuleValidationException("La fecha de fin del contrato debe ser posterior a la fecha de inicio");

            var contractor = new ContractorCompany
            {
                TenantId = tenantId,
                CompanyName = companyName,
                TaxId = taxId,
                ContactName = contactName,
                ContactEmail = contactEmail ?? throw new ArgumentNullException(nameof(contactEmail)),
                ContactPhone = contactPhone,
                Address = address ?? throw new ArgumentNullException(nameof(address)),
                ContractStartDate = contractStartDate,
                ContractEndDate = contractEndDate,
                IsActive = true
            };

            contractor.SetCreatedInfo(createdBy);

            return contractor;
        }

        public void Update(
            string companyName,
            string contactName,
            Email contactEmail,
            string contactPhone,
            Address address,
            DateTime? contractEndDate,
            Guid updatedBy)
        {
            CompanyName = companyName;
            ContactName = contactName;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            Address = address;
            ContractEndDate = contractEndDate;

            SetUpdatedInfo(updatedBy);
        }

        public void ExtendContract(DateTime newEndDate, Guid updatedBy)
        {
            if (newEndDate <= ContractStartDate)
                throw new BusinessRuleValidationException("La nueva fecha de fin debe ser posterior a la fecha de inicio");

            if (ContractEndDate.HasValue && newEndDate <= ContractEndDate.Value)
                throw new BusinessRuleValidationException("La nueva fecha de fin debe ser posterior a la fecha actual de fin");

            ContractEndDate = newEndDate;
            SetUpdatedInfo(updatedBy);
        }

        public void Deactivate(Guid userId)
        {
            IsActive = false;
            SetUpdatedInfo(userId);
        }

        public void Activate(Guid userId)
        {
            IsActive = true;
            SetUpdatedInfo(userId);
        }

        public bool IsContractExpired()
        {
            return ContractEndDate.HasValue && ContractEndDate.Value < DateTime.UtcNow.Date;
        }
    }
}