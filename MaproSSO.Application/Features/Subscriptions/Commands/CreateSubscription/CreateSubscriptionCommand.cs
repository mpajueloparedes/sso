using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Services;

namespace MaproSSO.Application.Features.Subscriptions.Commands.CreateSubscription
{
    [Authorize(Roles = "SuperAdmin")]
    public class CreateSubscriptionCommand : IRequest<Result<SubscriptionDto>>
    {
        public Guid TenantId { get; set; }
        public Guid PlanId { get; set; }
        public string BillingCycle { get; set; }
        public bool StartTrial { get; set; }
        public PaymentInfoDto Payment { get; set; }
    }

    public class PaymentInfoDto
    {
        public string PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public string CardholderName { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string Cvv { get; set; }
    }
}