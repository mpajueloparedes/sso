using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Features.Tenants.Commands.CreateTenant;

namespace MaproSSO.Application.Features.Tenants.Commands.UpdateTenant
{
    [Authorize(Permissions = "tenant.edit")]
    public class UpdateTenantCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string TradeName { get; set; }
        public string Industry { get; set; }
        public int EmployeeCount { get; set; }
        public AddressDto Address { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
    }
}
