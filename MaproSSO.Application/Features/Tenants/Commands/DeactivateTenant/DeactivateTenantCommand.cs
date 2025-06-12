using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Tenants.Commands.DeactivateTenant
{
    [Authorize(Roles = "SuperAdmin")]
    public class DeactivateTenantCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
        public string Reason { get; set; }
    }
}
