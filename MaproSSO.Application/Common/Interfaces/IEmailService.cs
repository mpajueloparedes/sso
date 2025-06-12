using System.Threading.Tasks;
using System.Collections.Generic;

namespace MaproSSO.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailMessage message);
        Task SendTemplateAsync(string templateName, string to, Dictionary<string, object> data);
        Task SendBulkAsync(List<EmailMessage> messages);
    }

    public class EmailMessage
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; } = true;
        public List<string> Cc { get; set; } = new();
        public List<string> Bcc { get; set; } = new();
        public List<EmailAttachment> Attachments { get; set; } = new();
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }
}