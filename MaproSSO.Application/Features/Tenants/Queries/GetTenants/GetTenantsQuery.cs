using MediatR;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Attributes;

namespace MaproSSO.Application.Features.Tenants.Queries.GetTenants
{
    [Authorize(Roles = "SuperAdmin")]
    public class GetTenantsQuery : IRequest<Result<PaginatedList<TenantDto>>>
    {
        public string SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string Industry { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string OrderBy { get; set; } = "CompanyName";
        public bool OrderDescending { get; set; } = false;
    }
}
