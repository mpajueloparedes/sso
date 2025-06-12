using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Services;

namespace MaproSSO.Application.Features.Subscriptions.Commands.ChangePlan
{
    [Authorize(Permissions = "subscription.manage")]
    public class ChangePlanCommand : IRequest<Result<SubscriptionDto>>
    {
        public Guid SubscriptionId { get; set; }
        public Guid NewPlanId { get; set; }
        public string NewBillingCycle { get; set; }
    }
}