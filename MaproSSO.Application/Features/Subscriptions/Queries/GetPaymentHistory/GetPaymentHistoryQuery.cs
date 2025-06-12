using System;
using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;
using MaproSSO.Application.Common.Services;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetPaymentHistory
{
    [Authorize(Permissions = "subscription.view")]
    public class GetPaymentHistoryQuery : IRequest<Result<PaginatedList<PaymentDto>>>
    {
        public Guid? TenantId { get; set; }
        public Guid? SubscriptionId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}