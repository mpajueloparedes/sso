using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Subscriptions.Commands.CancelSubscription
{
    [Authorize(Permissions = "subscription.manage")]
    public class CancelSubscriptionCommand : IRequest<r>
    {
        public Guid SubscriptionId { get; set; }
        public string Reason { get; set; }
        public bool ImmediateCancellation { get; set; }
    }
}