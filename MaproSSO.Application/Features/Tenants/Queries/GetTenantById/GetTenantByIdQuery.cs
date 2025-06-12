using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Tenants.Queries.GetTenantById
{
    [Authorize(Permissions = "tenant.view")]
    public class GetTenantByIdQuery : IRequest<Result<TenantDetailDto>>
    {
        public Guid Id { get; set; }
    }
}