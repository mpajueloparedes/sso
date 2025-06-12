using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetSubscriptionById
{
    [Authorize(Permissions = "subscription.view")]
    public class GetSubscriptionByIdQuery : IRequest<Result<SubscriptionDetailDto>>
    {
        public Guid Id { get; set; }
    }
}
