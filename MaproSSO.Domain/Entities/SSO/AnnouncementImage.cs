using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class AnnouncementImage : BaseEntity
    {
        public Guid AnnouncementId { get; private set; }
        public string ImageUrl { get; private set; }
        public string Description { get; private set; }
        public int SortOrder { get; private set; }
        public DateTime UploadedAt { get; private set; }
        public Guid UploadedBy { get; private set; }

        private AnnouncementImage() { }

        public static AnnouncementImage Create(
            Guid announcementId,
            string imageUrl,
            string description,
            int sortOrder,
            Guid uploadedBy)
        {
            return new AnnouncementImage
            {
                AnnouncementId = announcementId,
                ImageUrl = imageUrl,
                Description = description,
                SortOrder = sortOrder,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = uploadedBy
            };
        }

        public void UpdateSortOrder(int newOrder)
        {
            SortOrder = newOrder;
        }
    }
}