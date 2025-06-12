using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.ValueObjects
{
    public class DateRange : BaseValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private DateRange() { }

        private DateRange(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

        public static DateRange Create(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new DomainException("La fecha de inicio no puede ser mayor que la fecha de fin");

            return new DateRange(startDate, endDate);
        }

        public int GetDays() => (int)(EndDate - StartDate).TotalDays;

        public bool Contains(DateTime date) => date >= StartDate && date <= EndDate;

        public bool Overlaps(DateRange other)
        {
            return StartDate <= other.EndDate && EndDate >= other.StartDate;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }
    }
}