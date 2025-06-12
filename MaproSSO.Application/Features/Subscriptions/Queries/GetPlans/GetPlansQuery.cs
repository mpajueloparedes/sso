using MediatR;
using MaproSSO.Application.Common.Models;

namespace MaproSSO.Application.Features.Subscriptions.Queries.GetPlans
{
    public class GetPlansQuery : IRequest<Result<List<PlanDto>>>
    {
        public bool OnlyActive { get; set; } = true;
    }
}