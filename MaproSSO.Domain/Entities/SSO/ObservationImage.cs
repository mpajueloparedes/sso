using MaproSSO.Domain.Common;

namespace MaproSSO.Domain.Entities.SSO
{
    public class ObservationImage : BaseEntity
    {
        public Guid ObservationId { get; private set; }
        public string ImageUrl { get; private set; }
        public string Description { get; private set; }
        public int SortOrder { get; private set; }

        private ObservationImage() { }

        public static ObservationImage Create(
            Guid observationId,
            string imageUrl,
            string description,
            int sortOrder)
        {
            return new ObservationImage
            {
                ObservationId = observationId,
                ImageUrl = imageUrl,
                Description = description,
                SortOrder = sortOrder
            };
        }
    }
}