using MaproSSO.Domain.Entities.Tenant;

namespace MaproSSO.Domain.Specifications
{
    public class ActiveTenantsSpecification : BaseSpecification<Tenant>
    {
        public ActiveTenantsSpecification()
            : base(t => t.IsActive && !t.IsDeleted)
        {
            ApplyOrderBy(t => t.CompanyName);
        }
    }

    public class TenantByTaxIdSpecification : BaseSpecification<Tenant>
    {
        public TenantByTaxIdSpecification(string taxId)
            : base(t => t.TaxId == taxId)
        {
        }
    }

    public class TenantsWithActiveSubscriptionsSpecification : BaseSpecification<Tenant>
    {
        public TenantsWithActiveSubscriptionsSpecification()
            : base(t => t.IsActive)
        {
            AddInclude("Subscriptions");
        }
    }
}