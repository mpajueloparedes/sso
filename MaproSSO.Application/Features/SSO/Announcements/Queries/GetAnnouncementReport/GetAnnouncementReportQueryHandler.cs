//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using MediatR;
//using Microsoft.EntityFrameworkCore;
//using MaproSSO.Application.Common.Interfaces;
//using MaproSSO.Application.Common.Models;
//using MaproSSO.Domain.Enums;

//namespace MaproSSO.Application.Features.SSO.Announcements.Queries.GetAnnouncementReport
//{
//    public class GetAnnouncementReportQueryHandler : IRequestHandler<GetAnnouncementReportQuery, Result<AnnouncementReportDto>>
//    {
//        private readonly IApplicationDbContext _context;
//        private readonly ICurrentUserService _currentUser;

//        public GetAnnouncementReportQueryHandler(
//            IApplicationDbContext context,
//            ICurrentUserService currentUser)
//        {
//            _context = context;
//            _currentUser = currentUser;
//        }

//        public async Task<Result<AnnouncementReportDto>> Handle(
//            GetAnnouncementReportQuery request,
//            CancellationToken cancellationToken)
//        {
//            var query = _context.Announcements
//                .Include(a => a.Area)
//                .Include(a => a.CorrectiveActions)
//                .Where(a => a.TenantId == _currentUser.TenantId.Value &&
//                           a.ReportedAt >= request.DateFrom &&
//                           a.ReportedAt <= request.DateTo)
//                .AsQueryable();

//            if (request.AreaId.HasValue)
//            {
//                query = query.Where(a => a.AreaId == request.AreaId.Value);
//            }

//            var announcements = await query.ToListAsync(cancellationToken);

//            var report = new AnnouncementReportDto
//            {
//                DateFrom = request.DateFrom,
//                DateTo = request.DateTo,
//                TotalAnnouncements = announcements.Count,

//                // Por estado
//                PendingCount = announcements.Count(a => a.Status == AnnouncementStatus.Pending),
//                InProgressCount = announcements.Count(a => a.Status == AnnouncementStatus.InProgress),
//                CompletedCount = announcements.Count(a => a.Status == AnnouncementStatus.Completed),

//                // Por severidad
//                LowSeverityCount = announcements.Count(a => a.Severity == Severity.Low),
//                MediumSeverityCount = announcements.Count(a => a.Severity == Severity.Medium),
//                HighSeverityCount = announcements.Count(a => a.Severity == Severity.High),
//                CriticalSeverityCount = announcements.Count(a => a.Severity == Severity.Critical),