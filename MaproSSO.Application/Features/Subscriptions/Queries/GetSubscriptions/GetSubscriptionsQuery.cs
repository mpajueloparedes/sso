using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetSubscriptions
{
    [Authorize(Roles = "SuperAdmin")]
    public class GetSubscriptionsQuery : IRequest<Result<PaginatedList<SubscriptionListDto>>>
    {
        public Guid? TenantId { get; set; }
        public string Status { get; set; }
        public string PlanType { get; set; }
        public DateTime? ExpiringBefore { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "EndDate";
        public bool OrderDescending { get; set; } = false;
    }
}