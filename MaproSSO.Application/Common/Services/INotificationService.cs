using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MaproSSO.Application.Common.Services
{
    public interface INotificationService
    {
        Task SendNotificationAsync(NotificationRequest notification);
        Task SendBulkNotificationsAsync(List<NotificationRequest> notifications);
        Task SendSystemNotificationAsync(Guid tenantId, string subject, string message, NotificationType type);
        Task<List<NotificationDto>> GetUserNotificationsAsync(Guid userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
    }

    public class NotificationRequest
    {
        public Guid? UserId { get; set; }
        public Guid? TenantId { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public DateTime? ScheduledFor { get; set; }
    }

    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        Success,
        System
    }

    public enum NotificationChannel
    {
        InApp,
        Email,
        SMS,
        Push,
        All
    }
}