using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Tenants.Commands.CreateTenant
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateTenantCommand : IRequest<Result<TenantDto>>
    {
        public string CompanyName { get; set; }
        public string TradeName { get; set; }
        public string TaxId { get; set; }
        public string Industry { get; set; }
        public int EmployeeCount { get; set; }
        public AddressDto Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        // Admin User
        public string AdminFirstName { get; set; }
        public string AdminLastName { get; set; }
        public string AdminEmail { get; set; }
        public string AdminUsername { get; set; }
        public string AdminPassword { get; set; }

        // Subscription
        public Guid PlanId { get; set; }
        public string BillingCycle { get; set; }
    }

    public class AddressDto
    {
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
    }
}