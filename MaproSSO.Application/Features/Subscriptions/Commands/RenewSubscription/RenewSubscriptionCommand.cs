using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Services;
using MaproSSO.Application.Features.Subscriptions.Commands.CreateSubscription;

namespace MaproSSO.Application.Features.Subscriptions.Commands.RenewSubscription
{
    [Authorize(Permissions = "subscription.manage")]
    public class RenewSubscriptionCommand : IRequest<Result<SubscriptionDto>>
    {
        public Guid SubscriptionId { get; set; }
        public PaymentInfoDto Payment { get; set; }
    }
}