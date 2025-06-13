//using System;
//using System.Collections.Generic;
//using MediatR;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Application.Common.Attributes;

//namespace MaproSSO.Application.Features.Tenants.Queries.GetTenantSettings
//{
//    [Authorize(Permissions = "tenant.view")]
//    public class GetTenantSettingsQuery : IRequest<Result<List<TenantSettingDto>>>
//    {
//        public Guid TenantId { get; set; }
//    }
//}