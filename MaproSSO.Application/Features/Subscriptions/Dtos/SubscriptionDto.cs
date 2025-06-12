using System;
using MaproSSO.Application.Common.Models;
using MaproSSO.Application.Common.Mappings;
using MaproSSO.Domain.Entities.Subscription;
using MaproSSO.Domain.Enums;
using AutoMapper;

namespace MaproSSO.Application.Features.Subscriptions
{
    public class SubscriptionDto : AuditableDto, IMapFrom<Subscription>
    {
        public Guid TenantId { get; set; }
        public Guid PlanId { get; set; }
        public string PlanName { get; set; }
        public BillingCycle BillingCycle { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? NextBillingDate { get; set; }
        public bool AutoRenew { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsInGracePeriod { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Subscription, SubscriptionDto>()
                .ForMember(d => d.DaysRemaining, opt => opt.MapFrom(s => s.GetDaysRemaining()))
                .ForMember(d => d.IsInGracePeriod, opt => opt.MapFrom(s => s.IsInGracePeriod()));
        }
    }

    public class SubscriptionListDto : SubscriptionDto
    {
        public string TenantName { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }

    public class SubscriptionDetailDto : SubscriptionDto
    {
        public string TenantName { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public DateTime? GracePeriodEndDate { get; set; }
        public List<PlanFeatureDto> Features { get; set; }
        public List<SubscriptionHistoryDto> History { get; set; }
        public List<PaymentDto> RecentPayments { get; set; }
        public List<FeatureUsageDto> FeatureUsages { get; set; }
    }

    public class PlanDto : IMapFrom<Plan>
    {
        public Guid Id { get; set; }
        public string PlanName { get; set; }
        public PlanType PlanType { get; set; }
        public string Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public string MonthlyCurrency { get; set; }
        public decimal AnnualPrice { get; set; }
        public string AnnualCurrency { get; set; }
        public int TrialDays { get; set; }
        public bool IsActive { get; set; }
        public List<PlanFeatureDto> Features { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Plan, PlanDto>()
                .ForMember(d => d.MonthlyPrice, opt => opt.MapFrom(s => s.MonthlyPrice.Amount))
                .ForMember(d => d.MonthlyCurrency, opt => opt.MapFrom(s => s.MonthlyPrice.Currency))
                .ForMember(d => d.AnnualPrice, opt => opt.MapFrom(s => s.AnnualPrice.Amount))
                .ForMember(d => d.AnnualCurrency, opt => opt.MapFrom(s => s.AnnualPrice.Currency));
        }
    }

    public class PlanFeatureDto : IMapFrom<PlanFeature>
    {
        public string FeatureName { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureType { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class SubscriptionHistoryDto : IMapFrom<SubscriptionHistory>
    {
        public string Action { get; set; }
        public Guid? FromPlanId { get; set; }
        public Guid? ToPlanId { get; set; }
        public string FromBillingCycle { get; set; }
        public string ToBillingCycle { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedByName { get; set; }
    }

    public class PaymentDto : IMapFrom<Payment>
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public PaymentStatus Status { get; set; }
        public string FailureReason { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceUrl { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Payment, PaymentDto>()
                .ForMember(d => d.Amount, opt => opt.MapFrom(s => s.Amount.Amount))
                .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Amount.Currency));
        }
    }

    public class FeatureUsageDto
    {
        public string FeatureName { get; set; }
        public string FeatureCode { get; set; }
        public int CurrentUsage { get; set; }
        public int? Limit { get; set; }
        public decimal UsagePercentage { get; set; }
        public string ResetPeriod { get; set; }
    }
}