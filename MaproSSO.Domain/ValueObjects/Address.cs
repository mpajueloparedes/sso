using MaproSSO.Domain.Common;
using MaproSSO.Domain.Exceptions;

namespace MaproSSO.Domain.ValueObjects
{
    public class Address : BaseValueObject
    {
        public string Country { get; private set; }
        public string State { get; private set; }
        public string City { get; private set; }
        public string Street { get; private set; }
        public string PostalCode { get; private set; }

        private Address() { }

        private Address(string country, string state, string city, string street, string postalCode)
        {
            Country = country;
            State = state;
            City = city;
            Street = street;
            PostalCode = postalCode;
        }

        public static Address Create(string country, string state, string city, string street, string postalCode = null)
        {
            if (string.IsNullOrWhiteSpace(country))
                throw new DomainException("El país es requerido");

            if (string.IsNullOrWhiteSpace(state))
                throw new DomainException("El estado/provincia es requerido");

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("La ciudad es requerida");

            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("La dirección es requerida");

            return new Address(country, state, city, street, postalCode);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return State;
            yield return City;
            yield return Street;
            yield return PostalCode;
        }

        public override string ToString() => $"{Street}, {City}, {State}, {Country} {PostalCode}";
    }
}