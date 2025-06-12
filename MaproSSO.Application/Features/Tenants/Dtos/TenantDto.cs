using AutoMapper;
using MaproSSO.Application.Common.Mappings;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Features.Tenants.Commands.CreateTenant;
using MaproSSO.Domain.Entities.Tenant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace MaproSSO.Application.Features.Tenants
//{
//    public class TenantDto : AuditableDto, IMapFrom<Tenant>
//    {
//        public string CompanyName { get; set; }
//        public string TradeName { get; set; }
//        public string TaxId { get; set; }
//        public string Industry { get; set; }
//        public int EmployeeCount { get; set; }
//        public AddressDto Address { get; set; }
//        public string Phone { get; set; }
//        public string Email { get; set; }
//        public string Website { get; set; }
//        public string LogoUrl { get; set; }
//        public string TimeZone { get; set; }
//        public string Culture { get; set; }
//        public bool IsActive { get; set; }

//        public void Mapping(Profile profile)
//        {
//            profile.CreateMap<Tenant, TenantDto>()
//                .ForMember(d => d.Address, opt => opt.MapFrom(s => new AddressDto
//                {
//                    Country = s.Address.Country,
//                    State = s.Address.State,
//                    City = s.Address.City,
//                    Street = s.Address.Street,
//                    PostalCode = s.Address.PostalCode
//                }))
//                .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email.Value));
//        }
//    }

//    public class TenantDetailDto : TenantDto
//    {
//        public SubscriptionSummaryDto Subscription { get; set; }
//        public TenantStatisticsDto Statistics { get; set; }
//    }

//    public class SubscriptionSummaryDto
//    {
//        public string PlanName { get; set; }
//        public string Status { get; set; }
//        public DateTime EndDate { get; set; }
//        using System;
